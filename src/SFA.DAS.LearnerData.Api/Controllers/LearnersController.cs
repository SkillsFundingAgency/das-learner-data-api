using System.Net;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.LearnerData.Api.Controllers;

[Route("providers/{ukprn:long}/learners")]
[ApiVersion("1.0")]
[ApiController]
public class LearnersController: ControllerBase
{
    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetAll()
    {
        throw new NotImplementedException();
    }
    
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> Post()
    {
        throw new NotImplementedException();
    }
    
    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [Route("{uln:long}/agreements/{agreementId}/academicyears/{academicYear:int}")]
    public async Task<IActionResult> GetSingle(long uln, string agreementId, int academicYear)
    {
        throw new NotImplementedException();
    }
    
    [HttpPut]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [Route("{uln:long}/agreements/{agreementId}/academicyears/{academicYear:int}")]
    public async Task<IActionResult> Put()
    {
        throw new NotImplementedException();
    }
    
    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [Route("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        throw new NotImplementedException();
    }
    
    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [Route("academic-year")]
    public async Task<IActionResult> GetByAcademicYear([FromQuery] int academicYear, [FromQuery] int page = 1, [FromQuery] int pageSize = Int32.MaxValue)
    {
        throw new NotImplementedException();
    }
}