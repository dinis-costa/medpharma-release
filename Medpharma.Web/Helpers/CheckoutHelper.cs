using Medpharma.Web.Data.Entities;
using Medpharma.Web.Data.Repositories;
using Medpharma.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Medpharma.Web.Helpers
{
    public class CheckoutHelper : ICheckoutHelper
    {
        private readonly IMedicineRepository _medicineRepository;

        public CheckoutHelper(IMedicineRepository medicineRepository)
        {
            _medicineRepository = medicineRepository;
        }

        public async Task<CheckOutPrescriptionInfoViewModel> GetPrescriptionRemainingAsync(Appointment appointment)
        {
            List<Prescription> listMedicineStockOut = new();
            List<Prescription> listMedicineStockIn = new();

            foreach (var prescription in appointment.Prescriptions)
            {
                appointment.Prescriptions = appointment.Prescriptions.Where(a => a.MedicineId != null).ToList();
            }

            foreach (var presc in appointment.Prescriptions.ToList())
            {
                var m = await _medicineRepository.GetByIdAsync(presc.MedicineId.Value);

                if (m.Stock == 0)
                {
                    var pR = new Prescription
                    {
                        Number = presc.Number.Value,
                        Medicine = presc.Medicine,
                        Appointment = appointment,
                        ExpirationDate = presc.ExpirationDate,
                        Quantity = Math.Abs(m.Stock - presc.Quantity)

                    };

                    listMedicineStockOut.Add(pR);
                }

                if (m.Stock >= presc.Quantity)
                    listMedicineStockIn.Add(presc);

                if (m.Stock > 0 && m.Stock < presc.Quantity)
                {
                    if (presc.Quantity - m.Stock > 0)
                    {
                        var pR = new Prescription
                        {
                            Number = presc.Number.Value,
                            Medicine = presc.Medicine,
                            Appointment = appointment,
                            ExpirationDate = presc.ExpirationDate,
                            Quantity = Math.Abs(m.Stock - presc.Quantity)

                        };

                        listMedicineStockOut.Add(pR);
                    }

                    if (m.Stock - presc.Quantity < 0)
                    {
                        presc.Quantity = m.Stock;

                        listMedicineStockIn.Add(presc);
                    }

                }

            }

            return new CheckOutPrescriptionInfoViewModel
            {
                Appointment = appointment,
                ListMedicineStockIn = listMedicineStockIn,
                ListMedicineStockOut = listMedicineStockOut
            };



        }
    }
}
