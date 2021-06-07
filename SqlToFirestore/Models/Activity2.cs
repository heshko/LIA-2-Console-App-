using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Text;

namespace SqlToFirestore.Models
{
    [FirestoreData]
    public class Activity2
    {       
        [FirestoreProperty]
        public string Name { get; set; }
        [FirestoreProperty]
        public string FullName { get; set; }
        [FirestoreProperty]
        public DateTime Date { get; set; }
        [FirestoreProperty]
        public string Action { get; set; }
        [FirestoreProperty]
        public string Type { get; set; }
        [FirestoreProperty]
        public string Status { get; set; }
        [FirestoreProperty]
        public string Description { get; set; }
        [FirestoreProperty]
        public string Comment { get; set; }
        [FirestoreProperty]
        public string Log { get; set; }
        [FirestoreProperty]
        public string CompanyId { get; set; }

        [FirestoreProperty]
        public string ContactId { get; set; }

    }
}
