using MailKit.Net.Smtp;
using Medpharma.Web.Data.Entities;
using Medpharma.Web.Data.Repositories;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Medpharma.Web.Helpers
{
    public class MailHelper : IMailHelper
    {
        #region Fields & Constructor
        private readonly IConfiguration _configuration;
        private readonly ICustomerFileRepository _fileRepository;
        private readonly string ServerDirectory;

        public MailHelper(IConfiguration configuration, ICustomerFileRepository fileRepository)
        {
            _configuration = configuration;
            _fileRepository = fileRepository;
            ServerDirectory = Environment.CurrentDirectory + "\\ServerFiles\\";
        }
        #endregion

        public Response SendEmail(string to, string subject, string body)
        {
            var nameFrom = _configuration["Mail:NameFrom"];
            var from = _configuration["Mail:From"];
            var smtp = _configuration["Mail:Smtp"];
            var port = _configuration["Mail:Port"];
            var password = _configuration["Mail:Password"];

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(nameFrom, from));
            message.To.Add(new MailboxAddress(to, to));
            message.Subject = subject;

            var bodybuilder = new BodyBuilder
            {
                HtmlBody = body,
            };
            message.Body = bodybuilder.ToMessageBody();

            try
            {
                using (var client = new SmtpClient())
                {
                    client.Connect(smtp, int.Parse(port), false);
                    client.Authenticate(from, password);
                    client.Send(message);
                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.ToString()

                };
            }

            return new Response
            {
                IsSuccess = true
            };
        }

        //

        public async Task<Response> SendAppointmentEmail(Appointment appointment, byte type, List<Prescription>? precriptionsList)
        {
            var nameFrom = _configuration["Mail:NameFrom"];
            var from = _configuration["Mail:From"];
            var smtp = _configuration["Mail:Smtp"];
            var port = _configuration["Mail:Port"];
            var password = _configuration["Mail:Password"];

            var to = appointment.Customer.Email;
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(nameFrom, from));
            message.To.Add(new MailboxAddress(to, to));

            var bodybuilder = new BodyBuilder();

            switch (type)
            {
                case 1: // Create
                    message.Subject = $"Medpharma -- Appointment Scheduled -- {appointment.Date.ToString("dd/MMM/yyyy")} - {appointment.Timeslot.Slot}";
                    bodybuilder.HtmlBody = File.ReadAllText(@$"{ServerDirectory}\templates\email\create.html");
                    break;
                case 2: // Edit
                    message.Subject = $"Medpharma -- Appointment Altered -- {appointment.Date.ToString("dd/MMM/yyyy")}";
                    bodybuilder.HtmlBody = File.ReadAllText(@$"{ServerDirectory}\templates\email\edit.html");
                    break;
                case 3: // Delete
                    message.Subject = $"Medpharma -- Appointment Cancelled -- {appointment.Date.ToString("dd/MMM/yyyy")}";
                    bodybuilder.HtmlBody = File.ReadAllText(@$"{ServerDirectory}\templates\email\delete.html");
                    break;
                case 4: // Appt Receipt
                    message.Subject = $"Medpharma -- Appointment Concluded -- {appointment.Date.ToString("dd/MMM/yyyy")}";
                    bodybuilder.HtmlBody = File.ReadAllText(@$"{ServerDirectory}\templates\email\concluded.html");
                    break;
                case 5: // Shop
                    message.Subject = $"Medpharma -- Prescription Invoice  -- {appointment.Date.ToString("dd/MMM/yyyy")}";
                    bodybuilder.HtmlBody = File.ReadAllText(@$"{ServerDirectory}\templates\email\receipt.html");
                    break;
            }

            bodybuilder.HtmlBody = bodybuilder.HtmlBody.Replace("<%Speciality%>", $"{appointment.Speciality.Name}");
            bodybuilder.HtmlBody = bodybuilder.HtmlBody.Replace("<%Name%>", $"{appointment.Customer.FullName}");
            bodybuilder.HtmlBody = bodybuilder.HtmlBody.Replace("<%Doctor%>", $"{appointment.Doctor.FullName}");
            bodybuilder.HtmlBody = bodybuilder.HtmlBody.Replace("<%Date%>", $"{appointment.Date.Date}");
            if (appointment.TimeslotId != null)
            {
                bodybuilder.HtmlBody = bodybuilder.HtmlBody.Replace("<%Time%>", $"{appointment.Timeslot.Slot}");
            }
            else
                bodybuilder.HtmlBody = bodybuilder.HtmlBody.Replace("<%Time%>", $"Urgent Care");

            if (type == 4)
            {
                var invoice = await GenerateInvoice(appointment);

                if (appointment.Prescriptions.Count > 0) // 
                {
                    List<Prescription> medicine = appointment.Prescriptions.Where(a => a.MedicineId != null).ToList(); //
                    List<Prescription> exams = appointment.Prescriptions.Where(a => a.ExamId != null).ToList(); //

                    if (medicine.Count > 0)
                    {
                        var medicalPrescription = await GenerateMedicalPrescription(appointment);

                        if (medicalPrescription != string.Empty)
                            bodybuilder.Attachments.Add($@"{ServerDirectory}\files\{medicalPrescription}");
                    }

                    if (exams.Count > 0)
                    {
                        var examPrescription = await GenerateExamPrescription(appointment);

                        if (examPrescription.Count > 0)
                        {
                            foreach (var item in examPrescription)
                            {
                                bodybuilder.Attachments.Add($@"{ServerDirectory}\files\{item}");
                            }
                        }
                    }
                }

                bodybuilder.Attachments.Add($@"{ServerDirectory}\files\{invoice}");
            }

            if (type == 5)
            {
                var shopInvoice = await GeneratePrescriptionsInvoice(appointment, precriptionsList);
                bodybuilder.Attachments.Add($@"{ServerDirectory}\files\{shopInvoice}");
            }

            message.Body = bodybuilder.ToMessageBody();

            try
            {
                using (var client = new SmtpClient())
                {
                    client.Connect(smtp, int.Parse(port), false);
                    client.Authenticate(from, password);
                    client.Send(message);
                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.ToString()

                };
            }
            return new Response
            {
                IsSuccess = true
            };
        }

        public async Task<string> GenerateInvoice(Appointment appt)
        {
            var readTemplate = await File.ReadAllTextAsync(@$"{ServerDirectory}\templates\attachment\invoice.html");

            readTemplate = readTemplate.Replace("<%Reference%>", $"{appt.Id}");
            readTemplate = readTemplate.Replace("<%Date%>", $"{DateTime.Now}");
            readTemplate = readTemplate.Replace("<%Name%>", $"{appt.Customer.FullName}");
            readTemplate = readTemplate.Replace("<%Document%>", $"{appt.Customer.Document}");
            readTemplate = readTemplate.Replace("<%Email%>", $"{appt.Customer.Email}");
            readTemplate = readTemplate.Replace("<%Item%>", $"{appt.Doctor.Speciality.Name} (Appointment)");
            readTemplate = readTemplate.Replace("<%Price%>", $"{appt.Price.ToString("C2")}");
            readTemplate = readTemplate.Replace("<%Total%>", $"{appt.Price.ToString("C2")}");

            if (appt.MedicalScreeningId == null)
                readTemplate = readTemplate.Replace("<%Title%>", $"Invoice -- Scheduled Appointment");
            else
                readTemplate = readTemplate.Replace("<%Title%>", $"Invoice -- Urgent Care (ER)");

            string extension = ".html";
            Guid fileId = Guid.Empty;

            var guid = Guid.NewGuid().ToString();
            var file = $"{guid}{extension}";

            string path = Path.Combine(ServerDirectory, "files", file);

            fileId = Guid.Parse(guid);

            await File.WriteAllTextAsync(@$"{ServerDirectory}\files\{file}", readTemplate);

            var customerFile = new CustomerFile();
            customerFile.CustomerId = appt.CustomerId;
            customerFile.FileId = fileId;
            customerFile.Description = "Appointment Invoice";
            customerFile.FileExtension = extension;
            customerFile.Type = 2;
            customerFile.AppointmentId = appt.Id;

            await _fileRepository.CreateAsync(customerFile);

            return file;
        }

        public async Task<string> GenerateMedicalPrescription(Appointment appt)
        {
            if (appt.Prescriptions == null) return string.Empty;

            var readTemplate = await File.ReadAllTextAsync(@$"{ServerDirectory}\templates\attachment\prescription.html");

            readTemplate = readTemplate.Replace("<%Reference%>", $"{appt.Id}");
            readTemplate = readTemplate.Replace("<%Date%>", $"{DateTime.Now}");
            readTemplate = readTemplate.Replace("<%Name%>", $"{appt.Customer.FullName}");
            readTemplate = readTemplate.Replace("<%Document%>", $"{appt.Customer.Document}");
            readTemplate = readTemplate.Replace("<%Email%>", $"{appt.Customer.Email}");
            readTemplate = readTemplate.Replace("<%Title%>", $"Medication Prescription");

            foreach (var item in appt.Prescriptions)
            {
                if (item.Medicine != null)
                {
                    readTemplate = readTemplate.Replace("&nbsp;",
                    "<tr class=\"item\"> " +
                        $"<td>{item.Medicine.Name} - {item.Medicine.Description} - x{item.Quantity}</td>" +
                        $"<td>{item.Observations}</td>" +
                    "</tr>" +
                    "&nbsp;"
                    );
                }
            }

            string extension = ".html";
            Guid fileId = Guid.Empty;

            var guid = Guid.NewGuid().ToString();
            var file = $"{guid}{extension}";

            string path = Path.Combine(ServerDirectory, "files", file);

            fileId = Guid.Parse(guid);

            await File.WriteAllTextAsync(@$"{ServerDirectory}\files\{file}", readTemplate); // Write file.

            var customerFile = new CustomerFile();
            customerFile.CustomerId = appt.CustomerId;
            customerFile.FileId = fileId;
            customerFile.Description = "Medical Prescription";
            customerFile.FileExtension = extension;
            customerFile.Type = 3;
            customerFile.AppointmentId = appt.Id;

            await _fileRepository.CreateAsync(customerFile);

            return file;
        }

        public async Task<List<string>> GenerateExamPrescription(Appointment appt)
        {
            List<string> files = new();

            if (appt.Prescriptions == null) return files;

            foreach (var item in appt.Prescriptions)
            {
                if (item.Exam != null)
                {
                    var readTemplate = await File.ReadAllTextAsync(@$"{ServerDirectory}\templates\attachment\prescription.html");

                    readTemplate = readTemplate.Replace("<%Reference%>", $"{appt.Id}");
                    readTemplate = readTemplate.Replace("<%Date%>", $"{DateTime.Now}");
                    readTemplate = readTemplate.Replace("<%Name%>", $"{appt.Customer.FullName}");
                    readTemplate = readTemplate.Replace("<%Document%>", $"{appt.Customer.Document}");
                    readTemplate = readTemplate.Replace("<%Email%>", $"{appt.Customer.Email}");
                    readTemplate = readTemplate.Replace("<%Title%>", $"Exam Prescription");

                    readTemplate = readTemplate.Replace("&nbsp;",
                    "<tr class=\"item\"> " +
                        $"<td>{item.Exam.Name} ( CE: {item.Exam.Price.ToString("C2")})</td>" +
                        $"<td>{item.Observations}</td>" +
                    "</tr>" +
                    "&nbsp;"
                    );

                    string extension = ".html";
                    Guid fileId = Guid.Empty;

                    var guid = Guid.NewGuid().ToString();
                    var file = $"{guid}{extension}";

                    string path = Path.Combine(ServerDirectory, "files", file);

                    fileId = Guid.Parse(guid);

                    await File.WriteAllTextAsync(@$"{ServerDirectory}\files\{file}", readTemplate); // Write file.

                    var customerFile = new CustomerFile();
                    customerFile.CustomerId = appt.CustomerId;
                    customerFile.FileId = fileId;
                    customerFile.Description = "Exam Prescription";
                    customerFile.FileExtension = extension;
                    customerFile.Type = 4;
                    customerFile.AppointmentId = appt.Id;

                    await _fileRepository.CreateAsync(customerFile);

                    files.Add(file);
                }
            }

            return files;
        }

        //

        public async Task<Response> SendCartOrderEmail(Order order)
        {
            var nameFrom = _configuration["Mail:NameFrom"];
            var from = _configuration["Mail:From"];
            var smtp = _configuration["Mail:Smtp"];
            var port = _configuration["Mail:Port"];
            var password = _configuration["Mail:Password"];

            var to = order.Customer.Email;
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(nameFrom, from));
            message.To.Add(new MailboxAddress(to, to));

            var bodybuilder = new BodyBuilder();

            message.Subject = $"Medpharma -- Order Invoice -- {order.OrderDate.ToString("dd/MMM/yyyy")}";
            bodybuilder.HtmlBody = File.ReadAllText(@$"{ServerDirectory}\templates\email\receipt.html");

            bodybuilder.HtmlBody = bodybuilder.HtmlBody.Replace("<%Name%>", $"{order.Customer.FullName}");
            bodybuilder.HtmlBody = bodybuilder.HtmlBody.Replace("<%Date%>", $"{order.OrderDate}");

            var orderInvoice = await GenerateCartOrderInvoice(order);
            bodybuilder.Attachments.Add($@"{ServerDirectory}\files\{orderInvoice}");

            message.Body = bodybuilder.ToMessageBody();

            try
            {
                using (var client = new SmtpClient())
                {
                    client.Connect(smtp, int.Parse(port), false);
                    client.Authenticate(from, password);
                    client.Send(message);
                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.ToString()

                };
            }
            return new Response
            {
                IsSuccess = true
            };
        }
        public async Task<string> GenerateCartOrderInvoice(Order order)
        {
            var readTemplate = await File.ReadAllTextAsync(@$"{ServerDirectory}\templates\attachment\order.html");

            readTemplate = readTemplate.Replace("<%Reference%>", $"{order.Id}");
            readTemplate = readTemplate.Replace("<%Date%>", $"{DateTime.Now}");
            readTemplate = readTemplate.Replace("<%Name%>", $"{order.Customer.FullName}");
            readTemplate = readTemplate.Replace("<%Document%>", $"{order.Customer.Document}");
            readTemplate = readTemplate.Replace("<%Email%>", $"{order.Customer.Email}");
            readTemplate = readTemplate.Replace("<%Title%>", $"Order Invoice");
            readTemplate = readTemplate.Replace("<%Total%>", $"{order.CValue.ToString("C2")}");

            foreach (var item in order.Items)
            {
                readTemplate = readTemplate.Replace("&nbsp;",
                "<tr class=\"item\"> " +
                    $"<td>{item.Product.Name}</td>" +
                    $"<td>{item.Price.ToString("C2")}</td>" +
                    $"<td>{item.Quantity}</td>" +
                    $"<td>{item.Value.ToString("C2")}</td>" +
                "</tr>" +
                "&nbsp;");
            }

            string extension = ".html";
            Guid fileId = Guid.Empty;

            var guid = Guid.NewGuid().ToString();
            var file = $"{guid}{extension}";

            string path = Path.Combine(ServerDirectory, "files", file);

            fileId = Guid.Parse(guid);

            await File.WriteAllTextAsync(@$"{ServerDirectory}\files\{file}", readTemplate); // Write file.

            var customerFile = new CustomerFile();
            customerFile.CustomerId = order.Customer.Id;
            customerFile.FileId = fileId;
            customerFile.Description = "Order Invoice";
            customerFile.OrderId = order.Id;
            customerFile.FileExtension = extension;
            customerFile.Type = 6;

            await _fileRepository.CreateAsync(customerFile);

            return file;
        }

        //

        public async Task<string> GeneratePrescriptionsInvoice(Appointment appt, List<Prescription> prescriptionsList)
        {
            if (appt.Prescriptions == null) return string.Empty;

            var readTemplate = await File.ReadAllTextAsync(@$"{ServerDirectory}\templates\attachment\order.html");

            readTemplate = readTemplate.Replace("<%Reference%>", $"{appt.Id}");
            readTemplate = readTemplate.Replace("<%Date%>", $"{DateTime.Now}");
            readTemplate = readTemplate.Replace("<%Name%>", $"{appt.Customer.FullName}");
            readTemplate = readTemplate.Replace("<%Document%>", $"{appt.Customer.Document}");
            readTemplate = readTemplate.Replace("<%Email%>", $"{appt.Customer.Email}");
            readTemplate = readTemplate.Replace("<%Title%>", $"Fill Prescription Invoice");
            readTemplate = readTemplate.Replace("<%Total%>", $"{prescriptionsList.Select(c => c.Total).Sum().ToString("C2")}");

            foreach (var item in prescriptionsList)
            {
                if (item.Medicine != null)
                {
                    readTemplate = readTemplate.Replace("&nbsp;",
                    "<tr class=\"item\"> " +
                        $"<td>{item.Medicine.Name}</td>" +
                        $"<td>{item.Medicine.Price.ToString("C2")}</td>" +
                        $"<td class=\"text-right\">x{item.Quantity}</td>" +
                        $"<td>{item.Total.ToString("C2")}</td>" +
                    "</tr>" +
                    "&nbsp;"
                    );
                }
            }

            string extension = ".html";
            Guid fileId = Guid.Empty;

            var guid = Guid.NewGuid().ToString();
            var file = $"{guid}{extension}";

            string path = Path.Combine(ServerDirectory, "files", file);

            fileId = Guid.Parse(guid);

            await File.WriteAllTextAsync(@$"{ServerDirectory}\files\{file}", readTemplate); // Write file.

            var customerFile = new CustomerFile();
            customerFile.CustomerId = appt.Customer.Id;
            customerFile.FileId = fileId;
            customerFile.Description = "Prescription Invoice";
            customerFile.FileExtension = extension;
            customerFile.Type = 5;

            if (appt.IsRemaining == 1)
            {
                customerFile.AppointmentRemainingMedicinesId = appt.Id;
            }
            else
            {
                customerFile.AppointmentId = appt.Id;
            }

            await _fileRepository.CreateAsync(customerFile);

            return file;
        }

        //

        //public async Task<string> GenerateMedicalPrescriptionRemainig(Appointment appt, List<Prescription> prescriptionRamining)
        //{
        //    if (prescriptionRamining.Count == 0) return string.Empty;

        //    var readTemplate = await File.ReadAllTextAsync(@$"{ServerDirectory}\templates\attachment\prescription.html");

        //    readTemplate = readTemplate.Replace("<%DateTime%>", $"{DateTime.Now}");
        //    readTemplate = readTemplate.Replace("<%Name%>", $"{appt.Customer.FullName}");
        //    readTemplate = readTemplate.Replace("<%Phone%>", $"{appt.Customer.PhoneNumber}");
        //    readTemplate = readTemplate.Replace("<%Email%>", $"{appt.Customer.Email}");

        //    foreach (var item in prescriptionRamining)
        //    {
        //        if (item.Medicine != null)
        //        {
        //            readTemplate = readTemplate.Replace("&nbsp;",
        //            "<tr class=\"item\"> " +
        //                $"<td>{item.Medicine.Name}</td>" +
        //                $"<td>{item.Quantity}</td>" +
        //                $"<td>{item.Medicine.Price}</td>" +
        //                $"<td>{item.Observations}</td>" +
        //            "</tr>" +
        //            "&nbsp;"
        //            );
        //        }
        //    }

        //    string extension = ".html";
        //    Guid fileId = Guid.Empty;

        //    var guid = Guid.NewGuid().ToString();
        //    var file = $"{guid}{extension}";

        //    string path = Path.Combine(ServerDirectory, "files", file);

        //    fileId = Guid.Parse(guid);

        //    await File.WriteAllTextAsync(@$"{ServerDirectory}\files\{file}", readTemplate); // Write file.

        //    var customerFile = new CustomerFile();
        //    customerFile.CustomerId = appt.Customer.Id;
        //    customerFile.FileId = fileId;
        //    customerFile.Description = "Medical Prescription";
        //    customerFile.FileExtension = extension;
        //    customerFile.Type = 3;

        //    if (appt.IsRemaining == 1)
        //    {
        //        customerFile.AppointmentRemainingMedicinesId = appt.Id;
        //    }
        //    else
        //    {
        //        customerFile.AppointmentId = appt.Id;
        //    }

        //    await _fileRepository.CreateAsync(customerFile);

        //    return file;
        //}
    }
}
