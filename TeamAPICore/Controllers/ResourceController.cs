using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TeamAPICore.Context;
using TeamAPICore.Models;

namespace TeamAPICore.Controllers
{
    [Route("Resource")]
    public class ResourceController : AuthorizedApiController
    {
        private readonly ContractContext _contractContext;
        public ResourceController(ContractContext contractContext)
        {
            _contractContext = contractContext;
        }

        // http://localhost:58193/Resource/Contracts
        [HttpGet("Contracts")]
        public async Task<IActionResult> Contracts()
        {

            List<Contract> contracts = await _contractContext.GetContractsAsync(CRMContactID);
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

        // http://localhost:58193/Resource/Contracts/List
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
        public async Task<IActionResult> Contract(int id)
        {

            Contract contract = await _contractContext.GetContractAsync(id, CRMContactID);
            var result = new
            {
                User = UserInfo,
                Contract = contract
            };

            if (contract == null || contract.Id == 0)
            {
                return NotFound();
            }
            else
            {
                return Ok(result);

            }
        }
    }
}