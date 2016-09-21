using QM.CLITester.Entities;
using System;
using System.Collections.Generic;
using System.Xml;

namespace QM.CLITester.iTestIntegration.SourceProfilesReaders
{
    public class ProfileiTestReader : SourceProfileReader
    {

        public override Guid ConfigId { get { return new Guid("8cea6378-48fb-4d91-a9b5-9e0c3209321e"); } }
        ServiceActions _serviceActions = new ServiceActions();

        public override bool UseSourceItemId
        {
            get
            {
                return true;
            }
        }

        public override IEnumerable<SourceProfile> GetChangedItems(ref object updatedHandle)
        {
            string allProfilesResponse = _serviceActions.PostRequest("1011", null);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(allProfilesResponse);
            List<SourceProfile> sourceProfiles = new List<SourceProfile>();
            foreach (XmlNode nodeProfile in doc.DocumentElement.ChildNodes)
            {
                SourceProfile sourceProfile = new SourceProfile();
                sourceProfile.Name = nodeProfile.SelectSingleNode("Profile_Name").InnerText;
                sourceProfile.SourceId = nodeProfile.SelectSingleNode("Profile_ID").InnerText;

                ProfileExtensionSettings settings = new ProfileExtensionSettings();
                settings.GatewayIP = nodeProfile.SelectSingleNode("Profile_IP").InnerText;
                settings.GatewayPort = nodeProfile.SelectSingleNode("Profile_Port").InnerText;
                settings.SourceNumber = nodeProfile.SelectSingleNode("Profile_Src_Number").InnerText;


                sourceProfile.Settings = new ProfileSettings();

                sourceProfile.Settings.ExtendedSettings = new List<ExtendedProfileSetting>();

                sourceProfile.Settings.ExtendedSettings.Add(settings);
                sourceProfiles.Add(sourceProfile);
            }
            return sourceProfiles;
        }

    }
}
