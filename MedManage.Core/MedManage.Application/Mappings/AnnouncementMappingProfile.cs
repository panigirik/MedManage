using AutoMapper;
using MedManage.Application.DTOs;
using MedManage.Domain.Entities;
using Announcement = MedManage.Domain.Entities.Announcement;

namespace MedManage.Application.Mappings
{
    /// <summary>
    /// Предоставляет маппинг между сущностью <see cref="Announcement"/> и DTO <see cref="AnnouncementDTO"/>.
    /// </summary>
    public class AnnouncementMappingProfile : Profile
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AnnouncementMappingProfile"/>.
        /// Настраивает маппинг AutoMapper для сущности Announcement и её DTO.
        /// </summary>
        public AnnouncementMappingProfile()
        {
            CreateMap<Announcement, AnnouncementDTO>();

            CreateMap<AnnouncementDTO, Announcement>();
        }
    }
}