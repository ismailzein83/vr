using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public abstract class PackageExtendedSettings
    {
        //public abstract Guid ConfigId { get; }

        public virtual PackageExtendedSettingsEditorRuntime GetEditorRuntime()
        {
            return null;
        }

        public virtual void OnPackageAssignmentStarted(IPackageSettingAssignementStartedContext context)
        {

        }

        public virtual void OnPackageAssignmentExpired(IPackageSettingAssignementExpiredContext context)
        {

        }
        public abstract void ValidatePackageAssignment(IPackageSettingAssignementValidateContext context);
    }

    public abstract class PackageExtendedSettingsEditorRuntime
    {

    }

    public interface IPackageSettingAssignementStartedContext
    {
        long AccountId { get; }

        Account Account { get; }

        DateTime BED { get; }

        DateTime? EED { get; }
    }

    public interface IPackageSettingAssignementExpiredContext
    {
        long AccountId { get; }

        Account Account { get; }

        DateTime EED { get; }
    }

     public interface IPackageSettingAssignementValidateContext
    {
        long AccountId { get; }

        Account Account { get; }

        DateTime BED { get; }

        DateTime? EED { get; }
        bool IsValid { set; }
        string ErrorMessage { set; }
    }
     public class PackageSettingAssignementValidateContext : IPackageSettingAssignementValidateContext
     {
         public long AccountId { set; get; }

         public Account Account { set; get; }

         public DateTime BED { set; get; }

         public DateTime? EED { set; get; }
         public bool IsValid { set; get; }
         public string ErrorMessage { set; get; }
     }
}
