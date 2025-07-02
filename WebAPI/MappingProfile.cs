using AutoMapper;
using System.Linq;
using WebAPI.Data.Entities;
using WebAPI.Dtos.NationalMinority;
using WebAPI.Dtos.Topic;
using WebAPI.Dtos.CulturalHeritage;
using WebAPI.Dtos.Comment;
using WebAPI.Dtos.Log;

namespace WebAPI
{
	/// <summary>
	/// AutoMapper profile that defines all Entity ⇄ DTO mappings.
	/// </summary>
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			// ---- NationalMinority Mappings ----
			CreateMap<NationalMinority, NationalMinorityReadDto>();
			CreateMap<NationalMinorityCreateDto, NationalMinority>();

			// ---- Topic Mappings ----
			CreateMap<Topic, TopicReadDto>();
			CreateMap<TopicCreateDto, Topic>();

			// ---- CulturalHeritage Mappings ----
			// Entity → ReadDto (includes nested NationalMinority + Topic list)
			CreateMap<CulturalHeritage, CulturalHeritageReadDto>()
				.ForMember(dest => dest.Topics,
					opt => opt.MapFrom(src =>
						src.CulturalHeritageTopics
						   .Select(ct => ct.Topic)
						   .ToList()));
			// Log → LogReadDto
			CreateMap<Log, LogReadDto>();
			// CreateDto → Entity (ignore bridge collection; we'll handle topics manually in controller if needed)
			CreateMap<CulturalHeritageCreateDto, CulturalHeritage>()
				.ForMember(dest => dest.CulturalHeritageTopics, opt => opt.Ignore());

			// UpdateDto → Entity (ignore bridge collection for simplicity)
			CreateMap<CulturalHeritageUpdateDto, CulturalHeritage>()
				.ForMember(dest => dest.CulturalHeritageTopics, opt => opt.Ignore());

			// ---- Comment Mappings ----
			// Entity → ReadDto (flatten username)
			CreateMap<Comment, CommentReadDto>()
				.ForMember(dest => dest.Username,
					opt => opt.MapFrom(src => src.ApplicationUser.Username));

			// CreateDto → Entity
			CreateMap<CommentCreateDto, Comment>();
		}
	}
}
