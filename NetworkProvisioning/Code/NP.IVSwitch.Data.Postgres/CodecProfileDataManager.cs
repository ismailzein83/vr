using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.Postgres;
using Vanrise.Common;

namespace NP.IVSwitch.Data.Postgres
{
    class CodecProfileDataManager : BasePostgresDataManager, ICodecProfileDataManager
    {
        public CodecProfileDataManager()
            : base(GetConnectionStringName("NetworkProvisioningDBConnStringKey", "NetworkProvisioningDBConnString"))
        {

        }

        private CodecProfile CodecProfileMapper(IDataReader reader)
        {
            String CodecDefId = reader["ui_codec_display"] as string;
          //  CodecDefId = CodecDefId.Remove(CodecDefId.Length - 3);

            String Temp1 = CodecDefId.Replace("|-1", "");

        //    if (Temp1.EndsWith("@"))
          //      Temp1 = Temp1.Remove(Temp1.Length - 1);

            String[] Temp2 = Temp1.Split(':');
            List<int> CodecDefIdList = Temp2.Select(Int32.Parse).ToList();

             
            CodecProfile CodecProfile = new CodecProfile
            {
                CodecProfileId = (int)reader["codec_profile_id"],
                ProfileName = reader["profile_name"] as string,
                CreateDate = reader["create_date"].ToString(),
                CodecDefId = CodecDefIdList
               
            };

            return CodecProfile;
        }


        public List<CodecProfile> GetCodecProfiles()
        {
            String cmdText = "SELECT  codec_profile_id,profile_name,codec_string,create_date, ui_codec_display FROM codec_profiles;";
            return GetItemsText(cmdText, CodecProfileMapper,(cmd) =>
            {
            });
        }

        public bool Update(CodecProfile codecProfile, Dictionary<int, CodecDef> cachedCodecDef)
        {
            String codecString;
            String uiCodecDisplay;

            GetCodecDefParams(cachedCodecDef, codecProfile.CodecDefId, out codecString, out uiCodecDisplay);   

            String cmdText = @"UPDATE codec_profiles
	                             SET profile_name = @psgname, codec_string = @psgcodecstring, ui_codec_display = @psguicodecdisplay
                                 WHERE  codec_profile_id = @psgid and  NOT EXISTS(SELECT 1 FROM  trans_rules WHERE codec_profile_id != @psgid and profile_name = @psgname);";

            int recordsEffected = ExecuteNonQueryText(cmdText, (cmd) =>
            {
                cmd.Parameters.AddWithValue("@psgid", codecProfile.CodecProfileId);
                cmd.Parameters.AddWithValue("@psgname", codecProfile.ProfileName);
                cmd.Parameters.AddWithValue("@psgcodecstring", codecString);
                cmd.Parameters.AddWithValue("@psguicodecdisplay", uiCodecDisplay); 

 
            }
           );
            return (recordsEffected > 0);
        }

        public bool Insert(CodecProfile codecProfile, Dictionary<int, CodecDef> cachedCodecDef, out int insertedId)
        {
            object codecProfileId;
            String codecString;
            String uiCodecDisplay;

            GetCodecDefParams(cachedCodecDef, codecProfile.CodecDefId, out codecString, out uiCodecDisplay);          


            String cmdText = @"INSERT INTO codec_profiles(profile_name,codec_string,ui_codec_display)
	                             SELECT @psgname,@psgcodecstring, @psguicodecdisplay
	                             WHERE (NOT EXISTS(SELECT 1 FROM codec_profiles WHERE  profile_name = @psgname))
	                             returning  codec_profile_id;";

            codecProfileId = ExecuteScalarText(cmdText, (cmd) =>
            {
                cmd.Parameters.AddWithValue("@psgname", codecProfile.ProfileName);
                cmd.Parameters.AddWithValue("@psgcodecstring", codecString);
                cmd.Parameters.AddWithValue("@psguicodecdisplay", uiCodecDisplay); 

              }
            );

            insertedId = -1;
            if (codecProfileId != null)
            {
                insertedId = Convert.ToInt32(codecProfileId);
                return true;
            }
            else
                return false;

        }

        private void GetCodecDefParams(Dictionary<int, CodecDef> cachedCodecDef, List<int> CodecDefId, out String codecString, out String uiCodecDisplay)
        {
             StringBuilder codecStringBuilder = new StringBuilder("^^");
             StringBuilder idStringBuilder = new StringBuilder();
 
            Func<CodecDef, bool> filterExpression = (x) => (CodecDefId== null || CodecDefId.Contains(x.CodecId)); ;

            List<CodecDef> CodecDefList = cachedCodecDef.FindAllRecords(filterExpression).OrderBy(x => x.FsName).ToList();

            for (int i = 0; i < CodecDefList.Count(); i++)
            {
                codecStringBuilder.Append(":" + CodecDefList[i].FsName);

                if (idStringBuilder.Length != 0)
                    idStringBuilder.Append("|-1:" + CodecDefList[i].CodecId.ToString());
                else
                    idStringBuilder.Append(CodecDefList[i].CodecId.ToString());

            }

            if (idStringBuilder != null)
                idStringBuilder.Append( "|-1");

            codecString = codecStringBuilder.ToString();
            uiCodecDisplay = idStringBuilder.ToString();
             
             
        }

    }
}
