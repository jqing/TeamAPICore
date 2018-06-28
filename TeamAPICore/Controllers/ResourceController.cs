using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using TeamAPICore.Context;
using TeamAPICore.Models;
using TeamAPICore.Settings;

namespace TeamAPICore.Controllers
{
    [Route("Resource")]
    public class ResourceController : AuthorizedApiController
    {
        private readonly ContractContext _contractContext;
        private readonly IOptions<ApplicationOption> _option;
        public ResourceController(ContractContext contractContext, IOptions<ApplicationOption> option)
        {
            _contractContext = contractContext;
            _option = option;
        }

        // http://localhost:58193/Resource/Contracts/List
        [HttpGet("Contracts")]
        [HttpGet("Contracts/List")]
        public async Task<IActionResult> ContractList()
        {

            List<ContractSmall> contracts = await _contractContext.GetContractListAsync(CRMContactID);
            var result = new
            {
                User = UserInfo,
                Contracts = contracts
            };

            if (contracts.Count() == 0)
            {
                return NotFound();
            }
            else
            {
                return Ok(result);

            }
        }

        // http://localhost:58193/Resource/Contracts/12345
        [HttpGet("Contracts/{id}")]
        public IActionResult Contract(int id)
        {
            Contract contract = _contractContext.GetContract(id, CRMContactID);
            if (contract != null)
            {
                var documentBaaseUrl = $"http{(Request.IsHttps ? "s" : "")}://{Request.Host}{Request.PathBase}/Resource/Documents/";
                foreach (var doc in contract.APS)
                {
                    doc.URL = GetDownloadURL(doc, contract.Personid, documentBaaseUrl);
                }
                foreach (var doc in contract.Documents)
                {
                    doc.URL = GetDownloadURL(doc, contract.Personid, documentBaaseUrl);
                }
                foreach (var doc in contract.Correspondences)
                {
                    doc.URL = GetDownloadURL(doc, contract.Personid, documentBaaseUrl);
                }
                foreach (var m in contract.Communications)
                {
                    var hash = GetHash(m.Id.ToString(), CRMContactID);
                    m.URL = $"http{(Request.IsHttps ? "s" : "")}://{Request.Host}{Request.PathBase}/Resource/Communications/{m.Id}/{hash}";

                }
            }

            if (contract == null || contract.Id == 0)
            {
                return NotFound();
            }
            else
            {
                return Ok(new
                {
                    User = UserInfo,
                    Contract = contract
                });

            }
        }

        [HttpGet("Documents/{id}/{personid}/{hash}/{filename}")]
        public async Task<IActionResult> Document(int id, int personid, string hash, string fileName)
        {
            var filehash = await _contractContext.GetFileHashAsync(id);
            var checkhash = GetHash($"{id}/{personid}/{fileName}/{filehash}", filehash);

            if (personid > 0 && hash == checkhash)
            {
                var url = $"{_option.Value.DocServiceURL}/Document/Get/{id}/all/{personid}";
                var net = new WebClient();
                var data = net.DownloadData(url);
                var content = new MemoryStream(data);

                return File(content, GetContentType(fileName), fileName);
            }
            return NotFound();
        }

        [HttpGet("Communications/{id}/{hash}")]
        public async Task<IActionResult> Communication(int id, string hash)
        {
            var checkhash = GetHash(id.ToString(), CRMContactID);
            if (checkhash == hash)
            {
                var body = await _contractContext.GetEmailBodyAsync(id);
                return Ok(body);
            }
            return NotFound();
        }

        private string GetContentType(string fileName)
        {
            new FileExtensionContentTypeProvider().TryGetContentType(fileName, out string contentType);
            if (string.IsNullOrWhiteSpace(contentType))
                contentType = "application/octet-stream";
            return contentType;
        }

        private string GetDownloadURL(PersistedDocument doc,int personid,string DownloadURL)
        {
            var returnValue = string.Empty;
            if(doc != null)
            {
                var hash = GetHash($"{doc.Id}/{personid}/{doc.FileName}.{doc.Extension}/{doc.Hash}", doc.Hash);
                Uri baseUri = new Uri(DownloadURL);
                Uri downloadUri = new Uri(baseUri, $"{doc.Id}/{personid}/{hash}");
                returnValue = $"{downloadUri}/{Uri.EscapeDataString(doc.FileName + "." + doc.Extension)}";
            }
            return returnValue;
        }

        private String GetHash(String text, String key)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            Byte[] textBytes = encoding.GetBytes(text);
            Byte[] keyBytes = encoding.GetBytes(key);

            Byte[] hashBytes;

            using (HMACSHA256 hash = new HMACSHA256(keyBytes))
            {
                hashBytes = hash.ComputeHash(textBytes);
            }
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}