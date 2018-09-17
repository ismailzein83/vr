using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Data
{
    public interface ITranslationRuleDataManager: IDataManager
    {
        List<TranslationRule> GetTranslationRules();
        bool Update(TranslationRule TranslationRule);
        bool Insert(TranslationRule TranslationRule, out int insertedId);
        bool Delete(int translationRuleId);
    }
}
