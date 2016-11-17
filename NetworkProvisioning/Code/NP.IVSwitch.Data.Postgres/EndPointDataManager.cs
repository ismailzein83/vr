using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.Postgres;

namespace NP.IVSwitch.Data.Postgres
{
    public class EndPointDataManager : BasePostgresDataManager, IEndPointDataManager
    {public EndPointDataManager()
            : base(GetConnectionStringName("NetworkProvisioningDBConnStringKey", "NetworkProvisioningDBConnString"))
        {

        }

        private EndPoint RouteMapper(IDataReader reader)
        {
            

            EndPoint endPoint = new EndPoint();

            endPoint.EndPointId= (int)reader["user_id"];
            endPoint.AccountId = (int)reader["account_id"];
            endPoint.Description = reader["description"] as string;
            endPoint.GroupId = (int)reader["group_id"];

            endPoint.TariffId = GetReaderValue<int>(reader, "tariff_id");
            endPoint.LogAlias = reader["log_alias"] as string;
            endPoint.CodecProfileId = (int)reader["codec_profile_id"];
            endPoint.TransRuleId = (int)reader["trans_rule_id"];
           endPoint.CurrentState =  (State) GetReaderValue<Int16>(reader,"state_id");
            endPoint.ChannelsLimit = GetReaderValue<int>(reader,"channels_limit");
            endPoint.RouteTableId =  GetReaderValue<int>(reader,"route_table_id");
            endPoint.MaxCallDuration =  (int)reader["max_call_dura"];
            endPoint.RtpMode = (int)reader["rtp_mode"];
            endPoint.DomainId =  (Int16)reader["domain_id"];
            endPoint.SipLogin = reader["sip_login"] as string;
            endPoint.SipPassword = reader["sip_password"] as string;
            endPoint.Host = reader["host"] as string;
            endPoint.TechPrefix = reader["tech_prefix"] as string;
  

            return endPoint;
        }

        

        public List<EndPoint> GetEndPoints()
        {
 
            String cmdText = @"SELECT users.user_id,users.account_id,users.description,users.group_id,users.tariff_id,users.log_alias,users.codec_profile_id,
                                users.trans_rule_id,users.state_id,users.channels_limit,users.route_table_id,users.max_call_dura,users.rtp_mode,users.domain_id,
                                users.sip_login,users.sip_password,access_list.host,access_list.tech_prefix
                                FROM users LEFT JOIN access_list on access_list.user_id = users.user_id";
            return GetItemsText(cmdText,  RouteMapper, (cmd) =>
            {
            });
        }

        public bool SipUpdate(EndPoint endPoint)
        {

            int currentState, enableTrace ;

            MapEnum(endPoint, out currentState, out enableTrace);
 
            String cmdText = @"UPDATE users
	                             SET  description=@description,group_id=@group_id,tariff_id=@tariff_id,
                                   log_alias=@log_alias,codec_profile_id=@codec_profile_id,trans_rule_id=@trans_rule_id,state_id=@state_id,
                                   channels_limit=@channels_limit, route_table_id=@route_table_id,max_call_dura=@max_call_dura,rtp_mode=@rtp_mode,domain_id=@domain_id,
                                   sip_login=@sip_login,sip_password=@sip_password 
                                   WHERE  user_id = @user_id;";

            int recordsEffected = ExecuteNonQueryText(cmdText, (cmd) =>
            {
                cmd.Parameters.AddWithValue("@route_id", endPoint.EndPointId);                 
                cmd.Parameters.AddWithValue("@description", endPoint.Description);
                cmd.Parameters.AddWithValue("@group_id", endPoint.GroupId);
                cmd.Parameters.AddWithValue("@tariff_id", endPoint.TariffId);
                cmd.Parameters.AddWithValue("@log_alias", endPoint.LogAlias);
                cmd.Parameters.AddWithValue("@codec_profile_id", endPoint.CodecProfileId);
                cmd.Parameters.AddWithValue("@trans_rule_id", endPoint.TransRuleId);
                cmd.Parameters.AddWithValue("@state_id", currentState);
                cmd.Parameters.AddWithValue("@channels_limit", endPoint.ChannelsLimit);
                cmd.Parameters.AddWithValue("@route_table_id", endPoint.RouteTableId);
                cmd.Parameters.AddWithValue("@max_call_dura", endPoint.MaxCallDuration);
                cmd.Parameters.AddWithValue("@rtp_mode", endPoint.RtpMode);
                cmd.Parameters.AddWithValue("@domain_id", endPoint.DomainId);
                cmd.Parameters.AddWithValue("@sip_login", endPoint.SipLogin);
                cmd.Parameters.AddWithValue("@sip_password", endPoint.SipPassword);
              

            }
           );
            return (recordsEffected > 0);
        }

        public bool AclUpdate(EndPoint endPoint)
        {

            int currentState, enableTrace;

            MapEnum(endPoint, out currentState, out enableTrace);

            String cmdText = @"UPDATE access_list
	                             SET  description=@description,group_id=@group_id,tariff_id=@tariff_id,
                                   log_alias=@log_alias,codec_profile_id=@codec_profile_id,trans_rule_id=@trans_rule_id,state_id=@state_id,
                                   channels_limit=@channels_limit, route_table_id=@route_table_id,max_call_dura=@max_call_dura,rtp_mode=@rtp_mode,domain_id=@domain_id,
                                   host=@host,tech_prefix=@tech_prefix 
                                   WHERE  user_id = @user_id;";

            int recordsEffected = ExecuteNonQueryText(cmdText, (cmd) =>
            {
                cmd.Parameters.AddWithValue("@route_id", endPoint.EndPointId);
                cmd.Parameters.AddWithValue("@description", endPoint.Description);
                cmd.Parameters.AddWithValue("@group_id", endPoint.GroupId);
                cmd.Parameters.AddWithValue("@tariff_id", endPoint.TariffId);
                cmd.Parameters.AddWithValue("@log_alias", endPoint.LogAlias);
                cmd.Parameters.AddWithValue("@codec_profile_id", endPoint.CodecProfileId);
                cmd.Parameters.AddWithValue("@trans_rule_id", endPoint.TransRuleId);
                cmd.Parameters.AddWithValue("@state_id", currentState);
                cmd.Parameters.AddWithValue("@channels_limit", endPoint.ChannelsLimit);
                cmd.Parameters.AddWithValue("@route_table_id", endPoint.RouteTableId);
                cmd.Parameters.AddWithValue("@max_call_dura", endPoint.MaxCallDuration);
                cmd.Parameters.AddWithValue("@rtp_mode", endPoint.RtpMode);
                cmd.Parameters.AddWithValue("@domain_id", endPoint.DomainId);
                cmd.Parameters.AddWithValue("@host", endPoint.Host);
                cmd.Parameters.AddWithValue("@tech_prefix", endPoint.TechPrefix);


            }
           );

            // Update users table also
            if (SipUpdate(endPoint))
                return (recordsEffected > 0);
            else
                return false;
        }

        public bool SipInsert(EndPoint endPoint, out int insertedId)
        {
            object endPointId;
            int currentState, enableTrace;


            MapEnum(endPoint, out currentState, out enableTrace);
             

            String cmdText = @"INSERT INTO users(account_id,description,group_id,tariff_id,
                                   log_alias,codec_profile_id,trans_rule_id,state_id, channels_limit, route_table_id,max_call_dura,rtp_mode,domain_id,
                                    sip_login,sip_password)
	                             SELECT  @account_id, @description, @group_id, @tariff_id, @log_alias, @codec_profile_id, @trans_rule_id,@state_id,
                                 @channels_limit,  @route_table_id, @max_call_dura, @rtp_mode, @domain_id,@sip_login, @sip_password  
 	                             returning  user_id;";

            endPointId = ExecuteScalarText(cmdText, (cmd) =>
            {
                 cmd.Parameters.AddWithValue("@account_id", endPoint.AccountId);
                cmd.Parameters.AddWithValue("@description", endPoint.Description);
                cmd.Parameters.AddWithValue("@group_id", endPoint.GroupId);
                cmd.Parameters.AddWithValue("@tariff_id", endPoint.TariffId);
                cmd.Parameters.AddWithValue("@log_alias", endPoint.LogAlias);
                cmd.Parameters.AddWithValue("@codec_profile_id", endPoint.CodecProfileId);
                cmd.Parameters.AddWithValue("@trans_rule_id", endPoint.TransRuleId);
                cmd.Parameters.AddWithValue("@state_id", currentState);
                cmd.Parameters.AddWithValue("@channels_limit", endPoint.ChannelsLimit);
                cmd.Parameters.AddWithValue("@route_table_id", endPoint.RouteTableId);
                cmd.Parameters.AddWithValue("@max_call_dura", endPoint.MaxCallDuration);
                cmd.Parameters.AddWithValue("@rtp_mode", endPoint.Host);
                cmd.Parameters.AddWithValue("@domain_id", endPoint.DomainId);
                cmd.Parameters.AddWithValue("@sip_login", endPoint.SipLogin);
                cmd.Parameters.AddWithValue("@sip_password", endPoint.SipPassword);
 
            }
            );

            insertedId = -1;
            if (endPointId != null)
            {
                insertedId = Convert.ToInt32(endPointId);
                return true;
            }
            else
                return false;

        }

        public bool AclInsert(EndPoint endPoint, out int insertedId)
        {
            int endPointId;

            // insert into users and get user id
            SipInsert(endPoint, out endPointId);

            int currentState, enableTrace;

            MapEnum(endPoint, out currentState, out enableTrace);


            String cmdText = @"INSERT INTO users(user_id,account_id,description,group_id,tariff_id,
                                   log_alias,codec_profile_id,trans_rule_id,state_id, channels_limit, route_table_id,max_call_dura,rtp_mode,domain_id,
                                    host,tech_prefix)
	                             SELECT  @user_id,@account_id, @description, @group_id, @tariff_id, @log_alias, @codec_profile_id, @trans_rule_id,@state_id,
                                 @channels_limit,  @route_table_id, @max_call_dura, @rtp_mode, @domain_id,@host, @tech_prefix ";

             int recordsEffected = ExecuteNonQueryText(cmdText, (cmd) =>
            {
                cmd.Parameters.AddWithValue("@user_id", endPointId);
                cmd.Parameters.AddWithValue("@account_id", endPoint.AccountId);
                cmd.Parameters.AddWithValue("@description", endPoint.Description);
                cmd.Parameters.AddWithValue("@group_id", endPoint.GroupId);
                cmd.Parameters.AddWithValue("@tariff_id", endPoint.TariffId);
                cmd.Parameters.AddWithValue("@log_alias", endPoint.LogAlias);
                cmd.Parameters.AddWithValue("@codec_profile_id", endPoint.CodecProfileId);
                cmd.Parameters.AddWithValue("@trans_rule_id", endPoint.TransRuleId);
                cmd.Parameters.AddWithValue("@state_id", currentState);
                cmd.Parameters.AddWithValue("@channels_limit", endPoint.ChannelsLimit);
                cmd.Parameters.AddWithValue("@route_table_id", endPoint.RouteTableId);
                cmd.Parameters.AddWithValue("@max_call_dura", endPoint.MaxCallDuration);
                cmd.Parameters.AddWithValue("@rtp_mode", endPoint.Host);
                cmd.Parameters.AddWithValue("@domain_id", endPoint.DomainId);
                cmd.Parameters.AddWithValue("@host", endPoint.Host);
                cmd.Parameters.AddWithValue("@tech_prefix", endPoint.TechPrefix);

            }
            );

             insertedId = endPointId;

             return (recordsEffected > 0);


        }

        public bool Insert(EndPoint endPoint, out int insertedId)
        {
            if (endPoint.Host != null)
               return AclInsert(endPoint, out insertedId);
            else
              return SipInsert(endPoint, out insertedId);

        }

        public bool Update(EndPoint endPoint)
        {
            if (endPoint.Host != null)
                return AclUpdate(endPoint);
            else
                return SipUpdate(endPoint);

        }

        private void MapEnum(EndPoint endPoint, out int currentState, out int enableTrace)
        {
            var currentStateValue = Enum.Parse(typeof(State), endPoint.CurrentState.ToString());
            currentState = (int)currentStateValue;
            var enableTraceValue = Enum.Parse(typeof(Trace), endPoint.EnableTrace.ToString());
            enableTrace = (int)enableTraceValue;
 

        }

    }
}