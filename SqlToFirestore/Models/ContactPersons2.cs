using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SqlToFirestore.Models
{
    [FirestoreData]
   public  class ContactPersons2   
   {
        [FirestoreProperty]
        public string Name { get; set; }
        [FirestoreProperty]
        public string FirstName { get; set; }
        [FirestoreProperty]
        public string LastName { get; set; }
        [FirestoreProperty]
        public string Status { get; set; }
        [FirestoreProperty]
        public string JobTitle { get; set; }
        [FirestoreProperty]
        public string Email { get; set; }
        [FirestoreProperty]
        public string DirectTelephone { get; set; }
        [FirestoreProperty]
        public string Mandate { get; set; }
        [FirestoreProperty]
        public string PrimaryContact { get; set; }
        [FirestoreProperty]
        public string Comment { get; set; }
        [FirestoreProperty]
        public string Log { get; set; }
        [FirestoreProperty]    
        public string CompanyId { get; set; }

    }
}
