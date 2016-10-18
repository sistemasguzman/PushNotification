using Microsoft.AspNet.SignalR;
using PushNotification.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace PushNotification
{
    public class NotificationComponents
    {
        public void RegiterNotification(DateTime currentDate)
        {
            string connString = ConfigurationManager.ConnectionStrings["sqlConnectionStr"].ConnectionString;
            string sqlCommand = @"SELECT [ContactID],[ContactName],[ContactNo] FROM [dbo].[Contacts] WHERE [AddOn] > @AddedOn";
            using (SqlConnection cnn = new SqlConnection(connString))
            {
                SqlCommand cmm = new SqlCommand(sqlCommand, cnn);
                cmm.Parameters.AddWithValue(@"AddedOn", currentDate);
                if (cnn.State != System.Data.ConnectionState.Open)
                {
                    cnn.Open();
                }
                cmm.Notification = null;
                SqlDependency sqlDep = new SqlDependency(cmm);
                sqlDep.OnChange += sqlDep_OnChange;
                using (SqlDataReader reader = cmm.ExecuteReader())
                {

                }
            }
        }

        void sqlDep_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (e.Type == SqlNotificationType.Change)
            {
                SqlDependency dependency = sender as SqlDependency;
                dependency.OnChange -= sqlDep_OnChange;

                var notificationHub = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                notificationHub.Clients.All.notify("added");
                RegiterNotification(DateTime.Now);
            }
        }
        public List<Contacts> GetContacts(DateTime afterDate)
        {
            using (var context = new PushNotificationsContext())
            {
                return context.Contacts.Where(x => x.AddOn > afterDate).OrderByDescending(x => x.AddOn).ToList();
            }
        }

    }
}