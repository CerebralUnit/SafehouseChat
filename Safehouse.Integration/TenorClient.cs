using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TenorSharp;
using TenorSharp.Enums;
using TenorSharp.ResponseObjects;
using TenorSharp.SearchResults;
using Safehouse.Core;
namespace Safehouse.Integration
{
    public class TenorGifClient
    {
        public async Task< List<GifResult>> Search(string query)
        {
            var client = new TenorClient("5Z2X4KYG2HVW");

            Gif results = null;

            try{
                
                results = client.Search(query);
            }
            catch(Exception ex)
            {
                Debug.Write(ex.Message);
            }
           
            GifObject[] gifResult = null;

            if(results != null) 
                gifResult = results.GifResults;

            return gifResult.Select(x => new GifResult()
            {
                Width = x.Media[0][GifFormat.mp4].Dims[0],
                Height = x.Media[0][GifFormat.mp4].Dims[1],
                Tags = x.Tags.ToList(),
                Title = x.Title,
                Url = x.Media[0][GifFormat.mp4].Url.AbsoluteUri
            }).ToList();
        }
        public async Task<List<GifCategoryResult>> Categories()
        {
            var client = new TenorClient("5Z2X4KYG2HVW");
            Category results = client.Categories();

            return results.Tags.Select(x => new GifCategoryResult()
            {
                Image = x.Image.AbsoluteUri,
                Name = x.Name,
                Path = x.Path.AbsoluteUri,
                SearchTerm = x.SearchTerm
            }).ToList();
        }
        public async Task<List<string>> Trending()
        {

            var client = new RestClient("https://api.tenor.com/v1/");
            //var client = new TenorClient("5Z2X4KYG2HVW", new Locale("en_US") );

            client.AddDefaultParameter("key", "5Z2X4KYG2HVW", ParameterType.QueryString);
            client.AddDefaultParameter("locale", "en-US", ParameterType.QueryString);
            client.AddDefaultParameter("media_filter", "minimal", ParameterType.QueryString);

            var response = await client.ExecuteAsync(new RestRequest("trending"));
            var results = JsonConvert.DeserializeObject<Gif>(response.Content,new JsonSerializerSettings
            {
                Error = HandleDeserializationError
            });

            GifObject[] gifResult = null;

            if (results != null) 
                gifResult = results.GifResults; 

            return gifResult?.Select(x => x.Media[0][GifFormat.mp4].Url.AbsoluteUri).ToList();
        }


        public void HandleDeserializationError(object sender, ErrorEventArgs errorArgs)
        {
            var currentError = errorArgs.ErrorContext.Error.Message;
            errorArgs.ErrorContext.Handled = true;
        }
    }
}
