using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class LeasedLineRequestFlowManagement //To be completed
    {
        const string welcome = "34C4F53D-BB22-44DA-A938-ECEA5D528C0A";
        const string requestID = "6A46936E-70EB-4B0C-B3E1-12EA79EDDA77";
        const string address = "8C41CB22-2529-4716-9269-C579A9BAE25B";
        const string technicalTeam = "42A6FF3C-592A-4155-877B-D59B7628968B";
        const string services = "BE2B4318-F9AB-4A32-B793-76C64F929DBA";
        const string discount = "18800FBE-701C-46BC-B9A9-8BAF9441C873";
        const string paymentMethod = "4DBF157D-FC72-4B5E-A53F-07F8186EF6EF";
        const string print = "CE1AA066-2A3F-480E-856D-52AFED5493D3";
        const string site1MDFCabling = "A956FDBA-5577-4832-9470-CA3D6444357F";
        const string site1CabinetTeam = "E2C09497-68E4-4B4B-A86C-521303416385";
        const string site1DPTeam = "68F0805B-633D-4674-863F-EA36AE78A367";
        const string mic = "4DA0301B-A9DA-4521-B2A2-C5DF85E6F6C6";
        const string site2MDFCabling = "AEEE45D0-400D-4963-B1D0-488521D8B46E";
        const string site2CabinetTeam = "E264678C-D4BD-499E-9EC4-17CB7BAE65ED";
        const string site2DPTeam = "25933A59-4756-4CD6-B751-11E7DC9DDCF7";
        const string fiberTeam = "C91897C4-0452-4D03-893F-0DAF222F81D9";
        const string microwaveTeam = "54CFE6FD-32B0-4C94-B1CD-0D773ADB55AD";

        const string Completed = "1DFC2AE4-3E5D-4EFF-B37E-3BB8B423C49F";
        const string Completed2 = "12CD5791-FF95-4E92-9B64-E28F0D24684F";
       // const string NearbyNumbers = "01D567D3-44C5-432E-B487-B3E2957C12DC";


        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcome: nextStepId = requestID; break;
                case requestID: nextStepId = address; break;
                case address: nextStepId = technicalTeam; break;
            }
            return nextStepId;
        }
    }
}
