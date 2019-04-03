using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Deputies.BLL.Features.Analitics.Models;
using Deputies.BLL.Features.Analitics.Services;
using Deputies.BLL.Features.Deputies.Models;
using Deputies.BLL.Features.Deputies.Services;
using Deputies.BLL.Features.Index.Models;
using Deputies.Core.Interfaces;
using ParliamentaryInquiry.Core.Entities;

namespace Deputies.BLL.Features.Index.Services
{
    public class IndexService : IIndexService
    {
        private const int DeputiesPerPage = 10;

        private readonly IUnitOfWork unitOfWork;
        private readonly IAnaliticsService analiticsService;
        private readonly IDeputiesService deputiesService;
        private readonly IMapper mapper;

        public IndexService(IUnitOfWork unitOfWork, IAnaliticsService analiticsService, IDeputiesService deputiesService, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.analiticsService = analiticsService;
            this.deputiesService = deputiesService;
            this.mapper = mapper;
        }

        public async Task<IndexModel> GetIndexModelAsync()
        {
            var associations = await this.unitOfWork.GetRepository<DeputyAssociation>().GetAll();
            var inquries = await this.unitOfWork.GetRepository<Inqury>().GetAll();

            var allModels = await this.deputiesService.GetAllDeputiesAsync();

            var fullCount = allModels.Count();

            var models = allModels.OrderByDescending(x => inquries.Count(y => y.AuthorId == x.Id))
                .Take(5)
                .Select(x =>
                {
                    var model = this.mapper.Map<DeputyModel>(x);
                    model.Association = associations.FirstOrDefault(y => y.Id == x.AssociationId)?.Name;
                    return model;
                });


            var ratingItems = new List<DeputyRatingItem>();

            foreach (var deputy in models)
            {
                ratingItems.Add(new DeputyRatingItem()
                {
                    Deputy = deputy,
                    IndividualInquries = inquries.Count(y => y.AuthorId == deputy.Id),
                    CollectiveInquries = inquries.Count(y => y.CoauthorIds.Contains(deputy.Id))
                });
            }

            return new IndexModel()
            {
                Items = ratingItems,
                Associations = await this.analiticsService.GetAssociationsRating()
            };
        }
    }
}
