using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.LearnerData.Application.Commands.StopBackApprenticeship
{
    public class StopBackApprenticeshipCommand : IRequest
    {
        public long uln { get; set; }
        public long ApprenticeshipId { get; set; }
        public long LearnerDataId { get; set; }
    }
}
