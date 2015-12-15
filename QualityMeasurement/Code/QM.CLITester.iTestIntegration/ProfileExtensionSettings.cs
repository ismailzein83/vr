
using QM.CLITester.Entities;
using System;
using System.ComponentModel;
using System.Xml;
namespace QM.CLITester.iTestIntegration
{
    public enum ProfileType { [Description("std")]Voice = 1, [Description("sms")] SMS = 2 }


    public static class EnumExtensions
    {

        // This extension method is broken out so you can use a similar pattern with 
        // other MetaData elements in the future. This is your base method for each.
        public static T GetAttribute<T>(this Enum value) where T : Attribute
        {
            var type = value.GetType();
            var memberInfo = type.GetMember(value.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
            return (T)attributes[0];
        }

        // This method creates a specific call to the above method, requesting the
        // Description MetaData attribute.
        public static string ToName(this Enum value)
        {
            var attribute = value.GetAttribute<DescriptionAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }

    }


    public class ProfileExtensionSettings : QM.CLITester.Entities.ExtendedProfileSetting
    {
        ServiceActions _serviceActions = new ServiceActions();

        public ProfileType Type { get; set; }

        public string GatewayIP { get; set; }

        public string GatewayPort { get; set; }

        public string SourceNumber { get; set; }

        public int CallTime { get; set; }

        public int RingTime { get; set; }

        public string ITestProfileId { get; set; }


        public override void Apply(Entities.Profile profile)
        {
            //if (this.ITestProfileId == null)
            //{
            //    this.ITestProfileId = CreateProfile();
            //    if (this.ITestProfileId == null)
            //        throw new Exception("Could not create Profile at ITest");
            //}
            //UpdateProfile(profile);
        }

        private string CreateProfile()
        {
            string dummyProfileName = Guid.NewGuid().ToString();

            string createProfileResponse = _serviceActions.PostRequest("5010", String.Format("&name={0}&type={1}&ip={2}&port={3}&srcn={4}&call={5}&ring={6}", dummyProfileName, Type.ToName(), GatewayIP, GatewayPort, SourceNumber, CallTime, RingTime));
            CheckProfileResponse(createProfileResponse);
            string allProfilesResponse = _serviceActions.PostRequest("1011", null);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(allProfilesResponse);
            foreach (XmlNode nodeProfile in doc.DocumentElement.ChildNodes)
            {
                XmlNode nodeProfileName = nodeProfile.SelectSingleNode("Profile_Name");
                if (nodeProfileName.InnerText == dummyProfileName)
                {
                    XmlNode nodeProfileId = nodeProfile.SelectSingleNode("Profile_ID");
                    return nodeProfileId.InnerText;
                }
            }
            return null;
        }

        private void CheckProfileResponse(string createProfileResponse)
        {
            if (createProfileResponse == null || !createProfileResponse.Contains("<Status>Success</Status>"))
                throw new Exception(String.Format("Error when creating/updating Profile on ITest. Returned Response: {0}", createProfileResponse));
        }

        private void UpdateProfile(Profile Profile)
        {
            string createProfileResponse = _serviceActions.PostRequest("5010", String.Format("&pid={0}&name={1}&type={2}&ip={3}&port={4}&srcn={5}&call={6}&ring={7}", this.ITestProfileId, Profile.Name, Type.ToName(), GatewayIP, GatewayPort, SourceNumber, CallTime, RingTime));
            CheckProfileResponse(createProfileResponse);
        }

    }
}
