using System;
using Microsoft.WindowsAzure.Management.WebSites;
using Microsoft.WindowsAzure.Management.WebSites.Models;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Azure;
using Microsoft.Azure.Management.Resources;

namespace Poc.Setup
{
    class Program
    {
        static void Main(string[] args)
        {
            var resourceGroupName = "";
            var webSpaceName = "poc-azure-management-rg-NorthEuropewebspace";
            var webHostingPlanName = "poc-azure-management-sp";
            var webSiteName = "poc-azure-management-name";
            var creds = GetCredentials();

            //NeedResourceGroup(creds);
            NeedApplicationServicePlan(creds, webSpaceName, webHostingPlanName);
            NeedWebSite(creds, webSpaceName, webSiteName);
            DeployApi(creds);
            
            Console.ReadLine();
        }

        private static void NeedResourceGroup(SubscriptionCloudCredentials creds)
        {
            //using (var client = new ResourceManagementClient(creds))
            //{
            //    var parameters = new ResourceGroup();
            //    parameters.Location = "";
            //    var resourceGroup = client.ResourceGroups.CreateOrUpdate("", parameters);
            //}
        }

        private static void NeedApplicationServicePlan(SubscriptionCloudCredentials creds, string webSpaceName, string webHostingPlanName)
        {
            using(var client = new WebSiteManagementClient(creds))
            {
                try {
                    var response = client.WebHostingPlans.Get(webSpaceName, webHostingPlanName);
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("NotFound"))
                    {
                        var createParameters = new WebHostingPlanCreateParameters
                        {
                            Name = "",
                            WorkerSize = WorkerSizeOptions.Small,
                            NumberOfWorkers = 1,
                            SKU = SkuOptions.Standard
                        };
                        var response = client.WebHostingPlans.Create(webSpaceName, createParameters);
                    }
                }
            }
        }

        private static void NeedWebSite(SubscriptionCloudCredentials creds, string webSpaceName, string webSiteName)
        {
            using (var client = new WebSiteManagementClient(creds))
            {
                var getParameters = new WebSiteGetParameters { };
                try
                {
                    var getResponse = client.WebSites.Get(webSpaceName, webSiteName, getParameters);
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("NotFound"))
                    {
                        var createParameters = new WebSiteCreateParameters
                        {
                            Name = webSiteName,
                            ServerFarm = "poc-azure-management-sp"
                        };
                        var response = client.WebSites.Create(webSpaceName, createParameters);
                    }
                }
            }
        }

        private static void DeployApi(SubscriptionCloudCredentials creds)
        {
        }

        private static SubscriptionCloudCredentials GetCredentials()
        {
            var subscriptionId = Environment.GetEnvironmentVariable("poc-azure-management-subscription-id");
            var certificateKey = Environment.GetEnvironmentVariable("poc-azure-management-certificate-key");
            var certificateFullPath = Environment.GetEnvironmentVariable("poc-azure-management-certificate-path");

            var x509Cert = new X509Certificate2(certificateFullPath, certificateKey, X509KeyStorageFlags.MachineKeySet);

            return new CertificateCloudCredentials(subscriptionId, x509Cert);
        }
    }
}
