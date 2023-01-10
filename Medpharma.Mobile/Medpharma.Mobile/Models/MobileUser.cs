using System;
using System.Collections.Generic;
using System.Text;

namespace Medpharma.Mobile.Models
{
    public class MobileUser
    {
            public int sex { get; set; }
            public DateTime dateOfBirth { get; set; }
            public string weight { get; set; }
            public string height { get; set; }
            public bool hasInsurance { get; set; }
            public string medicalInfo { get; set; }
            public string address { get; set; }
            public string imageFullPath { get; set; }
            public object customerFiles { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string document { get; set; }
            public string fullName { get; set; }
            public string imageId { get; set; }
            public string id { get; set; }
            public string userName { get; set; }
            public string normalizedUserName { get; set; }
            public string email { get; set; }
            public string normalizedEmail { get; set; }
            public bool emailConfirmed { get; set; }
            public string passwordHash { get; set; }
            public string securityStamp { get; set; }
            public string concurrencyStamp { get; set; }
            public string phoneNumber { get; set; }
            public bool phoneNumberConfirmed { get; set; }
            public bool twoFactorEnabled { get; set; }
            public object lockoutEnd { get; set; }
            public bool lockoutEnabled { get; set; }
            public int accessFailedCount { get; set; }
    }
}
