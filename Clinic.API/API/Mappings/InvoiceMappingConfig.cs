using AutoMapper;
using Clinic.API.API.Dtos.DoctorDtos;
using Clinic.API.API.Dtos.InvoiceDtos;
using Clinic.API.Domain.Entities;
using Clinic.API.Domain.Enums;

namespace Clinic.API.API.Mappings
{
    public class InvoiceMappingConfig
    {

        public void Configure(Profile profile)
        {
            profile.CreateMap<CreateInvoiceDto, Invoice>()
                .ForMember(dest => dest.IssueDate,
                           opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.DueDate,
                           opt => opt.MapFrom(_ => DateTime.UtcNow.AddDays(30)))
                .ForMember(dest => dest.Status,
                           opt => opt.MapFrom(_ => InvoiceStatus.Issued));
            profile.CreateMap<CreateInvoiceItemDto, InvoiceItem>();

            // For creating DTOs from entities (Output)
            profile.CreateMap<Invoice, InvoiceDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            profile.CreateMap<InvoiceItem, InvoiceItemDto>();
        }






          
    }
}
