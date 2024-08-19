using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BeautyProductApp.Models;
using Newtonsoft.Json; 

namespace BeautyProductApp.Services
{
    public class ProductService : IProductService
    {
        public async Task<ResponseModel> GetProductsAsync(string endpoint)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(endpoint);
                    if (response.IsSuccessStatusCode)
                    {
                        var Result =await response.Content.ReadAsStringAsync();
                        var products = JsonConvert.DeserializeObject<ResponseModel>(Result);
                        return products;
                    }
                    else
                    {
                        return default(ResponseModel);
                    }
                                      
                }
            }
            catch (Exception ex)
            {
                
                return default(ResponseModel);
            }
        }
    }

   

public interface IProductService
    {
        Task<ResponseModel> GetProductsAsync(string endpoint);
    }
}
