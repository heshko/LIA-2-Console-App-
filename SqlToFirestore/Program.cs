
using SqlToFirestore.Entity;
using SqlToFirestore.Models;
using Google.Cloud.Firestore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SqlToFirestore
{
    class Program
    {
        static async Task  Main(string[] args)
        {

            //await AddNewCollection("ContactPersons", "LIA2");

            //await DeleteOldFiledInsideSubcollectionAndCollection();



            await AddNewCollection("x-BackendCopyContactPersons", "ContactPersons");




        }
        private static async Task GetDataFronSql()
        {

            ApplicationDbContext context = new ApplicationDbContext();
            var companies = context.Companies.ToList();
            var activities = context.Activities.ToList();
            var contactpersons = context.ContactPersons.ToList();
            var BusinessOpportunities = context.BusinessOpportunities.ToList();

            await AddDataInFireStor(companies, activities, contactpersons, BusinessOpportunities);
        }
        private static async Task AddDataInFireStor(List<Companies> companies, List<Activities> activities, List<ContactPersons> contactPersons, List<BusinessOpportunities> businessOpportunities)
        {
            FirestoreDb db = GetConnection();
            foreach (var company in companies)            {

                DocumentReference companyRef = db.Collection("Companies").Document();
                await companyRef.SetAsync(company);

                foreach (var activity in activities)
                {
                    if (activity.SystemIDOrganisation == company.SystemIDOrganisation)
                    {
                        activity.Date = activity.Date.ToUniversalTime();
                        DocumentReference activityRef = db.Collection("Companies").Document(companyRef.Id).Collection("Activities").Document();
                        await activityRef.SetAsync(activity);
                    }
                }

                foreach (var contactperson in contactPersons)
                {
                    if (contactperson.SystemIDOrganisation == company.SystemIDOrganisation)
                    {
                        DocumentReference contactPersonRef = db.Collection("Companies").Document(companyRef.Id).Collection("ContactPersons").Document();
                        await contactPersonRef.SetAsync(contactperson);
                    }
                }

                foreach (var businessOpportunity in businessOpportunities)
                {
                    if (businessOpportunity.SystemIDOrganisation == company.SystemIDOrganisation)
                    {
                        businessOpportunity.Created = businessOpportunity.Created.ToUniversalTime();
                        DocumentReference businessOpportunityRef = db.Collection("Companies").Document(companyRef.Id).Collection("BusinessOpportunities").Document();
                        await businessOpportunityRef.SetAsync(businessOpportunity);
                    }
                }
            }
        }
        private static FirestoreDb GetConnection()
        {
            string path = string.Concat(@"\source\repos\SqlToFirestore\SqlToFirestore", "/a.json");
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
            FirestoreDb db = FirestoreDb.Create("-sa-prod-311409");
            return db;
        }
        private static async Task RemoveIdInDocument(string collectionName)
        {
            var db = GetConnection();

            var docRef = await db.Collection("Companies").GetSnapshotAsync();

            foreach (var doc in docRef.Documents)
            {
                var collection = await db.Collection("Companies").Document(doc.Id).Collection(collectionName).GetSnapshotAsync();

                // "BussinessOpportunitiesId" make choise

                foreach (var item in collection.Documents)
                {
                    await db.Collection("Companies").Document(doc.Id).Collection("BusinessOpportunities").Document(item.Id).UpdateAsync("BussinessOpportunitiesId", FieldValue.Delete);

                }
            }
        }
        private static async Task UpdateIdInDocument(string collection)
        {
            var db = GetConnection();

            var docRef = await db.Collection("Companies").GetSnapshotAsync();

            foreach (var doc in docRef.Documents)
            {
                var documents = await db.Collection("Companies").Document(doc.Id).Collection(collection).GetSnapshotAsync();

                foreach (var item in documents.Documents)
                {
                    var act = item.ToDictionary();

                    string fullname = null;
                    string[] personName = null;
                    string personId = null;

                    if (act["FullName"] != null)
                    {
                        fullname = act["FullName"].ToString();
                        personName = fullname.Split(" ");
                        personId = act["SystemIDPerson"].ToString();
                    }

                    if (fullname != null && personId != null)
                    {
                        // contactPerson is Subcollection 
                        // Find person id witch has the same name as person
                        var person = db.Collection("Companies").Document(doc.Id).Collection("ContactPersons").WhereEqualTo("FirstName", personName[0]).WhereEqualTo("LastName", personName[1]);
                        var person2 = await person.GetSnapshotAsync();
                        string personId2 = null;

                        foreach (var pId in person2.Documents)
                        {
                            personId2 = pId.Id;
                        }
                        // BusinessOpportunities is Subcollection
                        await db.Collection("Companies").Document(doc.Id).Collection("BusinessOpportunities").Document(item.Id).UpdateAsync("SystemIDPerson", personId2);

                    }
                }
            }
        }
        private static async Task GetSubCollectionsAndAddINewCollection(string subCollectionName)
        {
            var db = GetConnection();
            var docRef = await db.Collection("x-BackendCopyCompany").GetSnapshotAsync();

            foreach (var item in docRef.Documents)
            {
                var snapShatDoc = await db.Collection("x-BackendCopyCompany").Document(item.Id).Collection(subCollectionName).GetSnapshotAsync();

                foreach (var doc in snapShatDoc.Documents)
                {
                    var docConvertToDictionary = doc.ToDictionary();
                    await db.Collection(subCollectionName).Document(doc.Id).SetAsync(docConvertToDictionary);
                }
            }
        }
        private static async Task AddNewIdFields()
        {
            var db = GetConnection();
            var docRef = await db.Collection("x-BackendCopyCompany").GetSnapshotAsync();

            foreach (var item in docRef.Documents)
            {
                string zipCodeValue = item.GetValue<string>("PostCodeZIP");
                string Organisationsnummer = item.GetValue<string>("Organisationsnummer");
                string Telephone = item.GetValue<string>("Telephone");
               
                    Dictionary<string, object> update = new Dictionary<string, object>
                    {
                        { "ZipCode", zipCodeValue },
                        { "OrganizationNumber", Organisationsnummer },
                        { "PhoneNumber", Telephone }
                    };
                    var document = await db.Collection("Companies").Document(item.Id).SetAsync(update, SetOptions.MergeAll);
            }
        }
        private static async Task DeleteOldFields()
        {
            var db = GetConnection();
            var docRef = await db.Collection("Companies").GetSnapshotAsync();

            foreach (var item in docRef.Documents)
            {
                string zipCodeValue = item.GetValue<string>("PostCodeZIP");
                string Organisationsnummer = item.GetValue<string>("Organisationsnummer");
                string Telephone = item.GetValue<string>("Telephone");

                Dictionary<string, object> update = new Dictionary<string, object>
                {
                    { "PostCodeZIP", FieldValue.Delete },
                    { "Organisationsnummer", FieldValue.Delete },
                    { "Telephone", FieldValue.Delete }
                };
                var document = await db.Collection("Companies").Document(item.Id).UpdateAsync(update);
            }
        }
        private static async Task AddNewFiledInsideSubcollectionAndCollection()
        {
            FirestoreDb db = GetConnection();
            var companies = await db.Collection("Companies").GetSnapshotAsync();

            foreach (var company in companies.Documents)
            {
                var activities = await db.Collection("Companies").Document(company.Id).Collection("Activities").GetSnapshotAsync();
                var businesses = await db.Collection("Companies").Document(company.Id).Collection("BusinessOpportunities").GetSnapshotAsync();
                var contactPersons = await db.Collection("Companies").Document(company.Id).Collection("ContactPersons").GetSnapshotAsync();

                if (activities.Count > 0)
                {
                    foreach (var activity in activities.Documents)
                    {
                        string SystemIDOrganisation = activity.GetValue<string>("SystemIDOrganisation");
                        string SystemIDPerson = activity.GetValue<string>("SystemIDPerson");                    

                        Dictionary<string, object> update = new Dictionary<string, object>
                        {
                            { "CompanyId", SystemIDOrganisation },
                            { "ContactId", SystemIDPerson }
                             
                        };
                        var document = await db.Collection("Companies").Document(company.Id).Collection("Activities").Document(activity.Id)
                            .SetAsync(update, SetOptions.MergeAll);
                    }
                }

                if (businesses.Count > 0)
                {
                    foreach (var business in businesses.Documents)
                    {
                        string SystemIDOrganisation = business.GetValue<string>("SystemIDOrganisation");
                        string SystemIDPerson = business.GetValue<string>("SystemIDPerson");

                        Dictionary<string, object> update = new Dictionary<string, object>
                        {
                            { "CompanyId", SystemIDOrganisation },
                            { "ContactId", SystemIDPerson }
                        };
                        var document = await db.Collection("Companies").Document(company.Id).Collection("BusinessOpportunities").Document(business.Id)
                            .SetAsync(update, SetOptions.MergeAll);
                    }
                }

                if (contactPersons.Count > 0)
                {
                    foreach (var contactPerson in contactPersons.Documents)
                    {
                        string SystemIDOrganisation = contactPerson.GetValue<string>("SystemIDOrganisation");
                       
                        Dictionary<string, object> update = new Dictionary<string, object>
                        {
                            { "CompanyId", SystemIDOrganisation },  
                        };
                        var document = await db.Collection("Companies").Document(company.Id).Collection("ContactPersons").Document(contactPerson.Id)
                            .SetAsync(update, SetOptions.MergeAll);
                    }
                }
            }

            var activitiesCollection = await db.Collection("Activities").GetSnapshotAsync();

            foreach (var activity in activitiesCollection.Documents)
            {
                string SystemIDOrganisation = activity.GetValue<string>("SystemIDOrganisation");
                string SystemIDPerson = activity.GetValue<string>("SystemIDPerson");


                Dictionary<string, object> update = new Dictionary<string, object>
                {
                    { "CompanyId", SystemIDOrganisation },
                    { "ContactId", SystemIDPerson }
                };
                var document = await db.Collection("Activities").Document(activity.Id).SetAsync(update, SetOptions.MergeAll);
            }

            var businessOpportunitiesCollection = await db.Collection("BusinessOpportunities").GetSnapshotAsync();

            foreach (var businessOpportunity in businessOpportunitiesCollection.Documents)
            {
                string SystemIDOrganisationBO = businessOpportunity.GetValue<string>("SystemIDOrganisation");
                string SystemIDPersonBO = businessOpportunity.GetValue<string>("SystemIDPerson");


                Dictionary<string, object> updateBO = new Dictionary<string, object>
                    {
                        { "CompanyId", SystemIDOrganisationBO },
                        { "ContactId", SystemIDPersonBO }
                    };
                var documentBO = await db.Collection("BusinessOpportunities").Document(businessOpportunity.Id).SetAsync(updateBO, SetOptions.MergeAll);
            }

            var contactPersonsCollection = await db.Collection("ContactPersons").GetSnapshotAsync();

            foreach (var contactPerson in contactPersonsCollection.Documents)
            {
                string SystemIDOrganisationCP = contactPerson.GetValue<string>("SystemIDOrganisation");

                Dictionary<string, object> updateCP = new Dictionary<string, object>
                    {
                        { "CompanyId", SystemIDOrganisationCP },

                    };
                var documentBO = await db.Collection("ContactPersons").Document(contactPerson.Id).SetAsync(updateCP, SetOptions.MergeAll);

            }
        }
        private static async Task DeleteOldFiledInsideSubcollectionAndCollection()
        {
            FirestoreDb db = GetConnection();
            var companies = await db.Collection("Companies").GetSnapshotAsync();

            foreach (var company in companies.Documents)
            {
                var activities = await db.Collection("Companies").Document(company.Id).Collection("Activities").GetSnapshotAsync();
                var businesses = await db.Collection("Companies").Document(company.Id).Collection("BusinessOpportunities").GetSnapshotAsync();
                var contactPersons = await db.Collection("Companies").Document(company.Id).Collection("ContactPersons").GetSnapshotAsync();

                if (activities.Count > 0)
                {
                    foreach (var activity in activities.Documents)
                    {  
                        Dictionary<string, object> update = new Dictionary<string, object>
                        {
                            { "SystemIDOrganisation",  FieldValue.Delete },
                            { "SystemIDPerson",  FieldValue.Delete }

                        };
                        var document = await db.Collection("Companies").Document(company.Id).Collection("Activities").Document(activity.Id)
                            .UpdateAsync(update);
                    }
                }

                if (businesses.Count > 0)
                {
                    foreach (var business in businesses.Documents)
                    {
                        Dictionary<string, object> update = new Dictionary<string, object>
                        {
                            { "SystemIDOrganisation",  FieldValue.Delete },
                            { "SystemIDPerson",  FieldValue.Delete }
                        };
                        var document = await db.Collection("Companies").Document(company.Id).Collection("BusinessOpportunities").Document(business.Id)
                            .UpdateAsync(update);
                    }
                }

                if (contactPersons.Count > 0)
                {
                    foreach (var contactPerson in contactPersons.Documents)
                    {
                        string SystemIDOrganisation = contactPerson.GetValue<string>("SystemIDOrganisation");

                        Dictionary<string, object> update = new Dictionary<string, object>
                        {
                            { "SystemIDOrganisation",  FieldValue.Delete },                          
                        };
                        var document = await db.Collection("Companies").Document(company.Id).Collection("ContactPersons").Document(contactPerson.Id)
                            .UpdateAsync(update);
                    }
                }
            }

            var activitiesCollection = await db.Collection("Activities").GetSnapshotAsync();

            foreach (var activity in activitiesCollection.Documents)
            {
                Dictionary<string, object> update = new Dictionary<string, object>
                {
                    { "SystemIDOrganisation",  FieldValue.Delete },
                    { "SystemIDPerson",  FieldValue.Delete }
                };

                var document = await db.Collection("Activities").Document(activity.Id).UpdateAsync(update);
            }

            var businessOpportunitiesCollection = await db.Collection("BusinessOpportunities").GetSnapshotAsync();

            foreach (var businessOpportunity in businessOpportunitiesCollection.Documents)
            {

                Dictionary<string, object> updateBO = new Dictionary<string, object>
                {
                    { "SystemIDOrganisation",  FieldValue.Delete },
                    { "SystemIDPerson",  FieldValue.Delete }
                };

                var documentBO = await db.Collection("BusinessOpportunities").Document(businessOpportunity.Id).UpdateAsync(updateBO);
            }

            var contactPersonsCollection = await db.Collection("ContactPersons").GetSnapshotAsync();

            foreach (var contactPerson in contactPersonsCollection.Documents)
            {   
                Dictionary<string, object> updateCP = new Dictionary<string, object>
                {
                   { "SystemIDOrganisation",  FieldValue.Delete }
                };
                var documentBO = await db.Collection("ContactPersons").Document(contactPerson.Id).UpdateAsync(updateCP);

            }
        }
        private static async Task AddNewCollection(string collecitionName, string newCollectitonName)
        {
            var db = GetConnection();

            var documents = await db.Collection(collecitionName).GetSnapshotAsync();

            foreach (var document in documents.Documents)
            {
                var entity = document.ConvertTo<ContactPersons2>();
                await db.Collection(newCollectitonName).Document(document.Id).SetAsync(entity);
            }
        }
        private static async Task AddNewCollectionWithSubCollection(string collecitionName, string newCollectitonName)
        {
            var db = GetConnection();

            var documents = await db.Collection(collecitionName).GetSnapshotAsync();

            foreach (var document in documents.Documents)
            {
                var entity = document.ConvertTo<Companies2>();
                await db.Collection(newCollectitonName).Document(document.Id).SetAsync(entity);

                var activities = await db.Collection(collecitionName).Document(document.Id).Collection("x-BackendCopyActivities").GetSnapshotAsync();
                var businessOpportunities = await db.Collection(collecitionName).Document(document.Id).Collection("x-BackendCopyBusinessOpportunities").GetSnapshotAsync();
                var contactPersons = await db.Collection(collecitionName).Document(document.Id).Collection("x-BackendCopyContactPersons").GetSnapshotAsync();

                if (activities.Count > 0)
                {
                    foreach (var activity in activities.Documents)
                    {
                        var act = activity.ConvertTo<Activity2>();

                        await db.Collection(newCollectitonName).Document(document.Id).Collection("Activities").Document(activity.Id).SetAsync(act);
                    }
                   
                }

                if (businessOpportunities.Count > 0)
                {
                    foreach (var businessOpportunity in businessOpportunities.Documents)
                    {
                        var business = businessOpportunity.ConvertTo<BusinessOpportunities2>();

                        await db.Collection(newCollectitonName).Document(document.Id).Collection("BusinessOpportunities").Document(businessOpportunity.Id).SetAsync(business);
                    }

                }

                if (contactPersons.Count > 0)
                {
                    foreach (var contactPerson in contactPersons.Documents)
                    {
                        var cP = contactPerson.ConvertTo<ContactPersons2>();

                        await db.Collection(newCollectitonName).Document(document.Id).Collection("ContactPersons").Document(contactPerson.Id).SetAsync(cP);
                    }

                }
            }
        }
                 
    }
}
