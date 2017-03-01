using Demo.Module.Data;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;

namespace Demo.Module.Business
{
  public  class CityManager
    {
      public IDataRetrievalResult<CityDetails> GetFilteredCities(Vanrise.Entities.DataRetrievalInput<CityQ> input)
      {
          ICityDataManager dataManager = CityFactory.GetDataManager<ICityDataManager>();
          IEnumerable<Demo.Module.Entities.City> cities = dataManager.GetCities();
          var allCities = cities.ToDictionary(c => c.Id, c => c);

          Func<Demo.Module.Entities.City, bool> filterExpression = (prod) =>
              {
                  if (input.Query.Name != null && !prod.Name.ToLower().Contains(input.Query.Name.ToLower()))
                      return false;
                  return true;
              };
              




          return DataRetrievalManager.Instance.ProcessResult(input, allCities.ToBigResult(input, filterExpression, CityDetailMapper));
      }


      public Vanrise.Entities.InsertOperationOutput<CityDetails> AddCity(Demo.Module.Entities.City city)
      {
          Vanrise.Entities.InsertOperationOutput<CityDetails> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<CityDetails>();

          insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
          insertOperationOutput.InsertedObject = null;

          int cityId = -1;

          ICityDataManager dataManager = CityFactory.GetDataManager<ICityDataManager>();
          bool insertActionSucc = dataManager.Insert(city, out cityId);
          if (insertActionSucc)
          {
            
              insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
              city.Id = cityId;
              insertOperationOutput.InsertedObject = CityDetailMapper(city);
          }
          else
          {
              insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
          }

          return insertOperationOutput;
      }

      public Demo.Module.Entities.City GetCity(int Id)
      {
          ICityDataManager dataManager = CityFactory.GetDataManager<ICityDataManager>();
          string CityName;
          bool selectedName= dataManager.GetCitie(Id,out CityName);
          Demo.Module.Entities.City c = new Demo.Module.Entities.City()
          {
              Id=Id,
              Name=CityName
                
          };


          return c;
      }
      public Vanrise.Entities.UpdateOperationOutput<CityDetails> UpdateCity(Demo.Module.Entities.City city)
      {
          ICityDataManager dataManager = CityFactory.GetDataManager<ICityDataManager>();

          bool updateActionSucc = dataManager.Update(city);
          Vanrise.Entities.UpdateOperationOutput<CityDetails> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<CityDetails>();

          updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
          updateOperationOutput.UpdatedObject = null;

          if (updateActionSucc)
          {
              
              updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
              updateOperationOutput.UpdatedObject = CityDetailMapper(city);
          }
          else
          {
              updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
          }
          return updateOperationOutput;
      }


      public CityDetails CityDetailMapper(Demo.Module.Entities.City city)
      {
          CityDetails cityDetail = new CityDetails();

          
          

          cityDetail.Entity = city;
         
          return cityDetail;
      }





    }
}
