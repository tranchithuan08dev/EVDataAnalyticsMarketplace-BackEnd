﻿using EV.AdminService.API.DTOs.DataModels;
using EV.AdminService.API.Repositories.Interfaces;
using EV.AdminService.API.Services.Interfaces;

namespace EV.AdminService.API.Services.Implements
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrganizationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<OrganizationDetailDTO>> GetOrganizationsAsync(CancellationToken ct = default)
        {
            return await _unitOfWork.OrganizationRepository.GetOrganizationsAsync(ct).ConfigureAwait(false);
        }
    }
}
