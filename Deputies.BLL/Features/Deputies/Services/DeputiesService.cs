using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Deputies.BLL.Features.Analitics.Models;
using Deputies.BLL.Features.Deputies.Models;
using Deputies.BLL.Shared.Models;
using Deputies.Core.Interfaces;
using ParliamentaryInquiry.Core.Entities;

namespace Deputies.BLL.Features.Deputies.Services
{
    public class DeputiesService : IDeputiesService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public DeputiesService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<DeputiesByAssociationModel> GetDeputiesByAssociationAsync(string associationId)
        {
            var association = this.mapper
                .Map<DeputyAssociationModel>(await this.unitOfWork
                .GetRepository<DeputyAssociation>()
                .GetById(associationId));

            var deputiesByAssociation = (await this.GetAllDeputiesAsync())
                .Where(x => x.AssociationId == associationId)
                .Select(this.mapper.Map<DeputyModel>);

            return new DeputiesByAssociationModel()
            {
                Association = association,
                Deputies = deputiesByAssociation
            };
        }

        public async Task<DeputiesByPartyModel> GetDeputiesByPartyAsync(string partyId)
        {
            var party = this.mapper
                .Map<PoliticalPartyModel>(await this.unitOfWork
                .GetRepository<PoliticalParty>()
                .GetById(partyId));

            var deputies = (await this.unitOfWork
                .GetRepository<MultiMemberDeputy>()
                .SearchFor(x => x.PartyId == partyId))
                .Select(this.mapper.Map<DeputyModel>);

            return new DeputiesByPartyModel()
            {
                Party = party,
                Deputies = deputies
            };
        }

        public async Task<DeputiesByRegionModel> GetDeputiesByRegionAsync(string regionName)
        {
            var result = new DeputiesByRegionModel()
            {
                RegionName = regionName
            };

            var allRegions = await this.unitOfWork.GetRepository<AdministrativeUnit>().GetAll();
            var region = allRegions.First(x => x.Name == regionName);
            var subRegions = allRegions.Where(x => x.ParrentId == region.Id);
            if(subRegions.Any())
            {
                foreach(var subRegion in subRegions)
                {
                    var subRegionConstituencies = await this.unitOfWork
                        .GetRepository<Constituency>()
                        .SearchFor(x => x.RegionId == subRegion.Id);

                    var subRegionConstituenciesIds = subRegionConstituencies.Select(x => x.Id);

                    var subRegionDeputies = await this.unitOfWork
                        .GetRepository<SingleMemberDeputy>()
                        .SearchFor(x => subRegionConstituenciesIds.Contains(x.ConstituencyId));

                    var subRegionModel = new DeputiesByRegionModel()
                    {
                        RegionName = subRegion.Name
                    };

                    foreach(var deputy in subRegionDeputies)
                    {
                        var accordingConstituency = this.mapper.Map<ConstituencyModel>(subRegionConstituencies.First(x => x.Id == deputy.ConstituencyId));
                        var mappedDeputy = this.mapper.Map<SingleMemberDeputyModel>(deputy);
                        subRegionModel.ByRegion.Add(new DeputyByConstituencyModel()
                        {
                            Constituency = accordingConstituency,
                            Deputy = mappedDeputy
                        });
                    }

                    result.BySubregion.Add(subRegionModel);
                }
            }

            var constituencies = await this.unitOfWork
                        .GetRepository<Constituency>()
                        .SearchFor(x => x.RegionId == region.Id);

            var constituenciesIds = constituencies.Select(x => x.Id);

            var deputies = await this.unitOfWork
                .GetRepository<SingleMemberDeputy>()
                .SearchFor(x => constituenciesIds.Contains(x.ConstituencyId));

            foreach (var deputy in deputies)
            {
                var accordingConstituency = this.mapper.Map<ConstituencyModel>(constituencies.First(x => x.Id == deputy.ConstituencyId));
                var mappedDeputy = this.mapper.Map<SingleMemberDeputyModel>(deputy);
                result.ByRegion.Add(new DeputyByConstituencyModel()
                {
                    Constituency = accordingConstituency,
                    Deputy = mappedDeputy
                });
            }

            return result;
        }

        public async Task<IEnumerable<DeputiesCountByRegionModel>> GetDeputiesCountByRegionAsync()
        {
            var regions = await this.unitOfWork.GetRepository<AdministrativeUnit>().GetAll();
            var constituencies = await this.unitOfWork.GetRepository<Constituency>().GetAll();
            var singleMemberDeputies = await this.unitOfWork.GetRepository<SingleMemberDeputy>().GetAll();

            var groupped = singleMemberDeputies.GroupBy(x => x.ConstituencyId).GroupBy(x => regions.FirstOrDefault(r => r.Id == constituencies.FirstOrDefault(c => c.Id == x.Key)?.RegionId)).Select(x => new Pair<AdministrativeUnit, int>(x.Key, x.Count()));

            var groupedByRegions = groupped.Where(x => x.First.ParrentId == null).ToList();

            foreach (var group in groupped)
            {
                if (group.First.ParrentId != null)
                {
                    groupedByRegions.First(x => x.First.Id == group.First.ParrentId).Second += group.Second;
                }
            }

            return groupedByRegions.Select(x => new DeputiesCountByRegionModel()
            {
                Region = x.First.Name,
                Count = x.Second
            });
        }

        public async Task<IEnumerable<Deputy>> GetAllDeputiesAsync()
        {
            var singleMemberDeputies = await this.unitOfWork
                .GetRepository<SingleMemberDeputy>()
                .GetAll();

            var partyMemberDeputies = await this.unitOfWork
                .GetRepository<MultiMemberDeputy>()
                .GetAll();

            return singleMemberDeputies.Cast<Deputy>().Concat(partyMemberDeputies.Cast<Deputy>());
        }

        public async Task<DeputyInfoModel> GetDeputyInfo(string deputyId)
        {
            var singleDeputy = await this.unitOfWork.GetRepository<SingleMemberDeputy>().GetById(deputyId);
            var multyDeputy = await this.unitOfWork.GetRepository<MultiMemberDeputy>().GetById(deputyId);

            var deputyModel = new DeputyModel();

            if (singleDeputy != null)
            {
                deputyModel.Id = singleDeputy.Id;
                deputyModel.Name = singleDeputy.Name;
                deputyModel.Email = singleDeputy.Email;
                deputyModel.IsActive = singleDeputy.IsActive;
                deputyModel.Link = singleDeputy.Link;
                deputyModel.PhotoLink = singleDeputy.PhotoLink;
                deputyModel.Position = singleDeputy.Position;

                var constituency = await this.unitOfWork.GetRepository<Constituency>().GetById(singleDeputy.ConstituencyId);
                deputyModel.ChosenBy = "Виборчому округу №" + constituency.Number;

                var association = await this.unitOfWork.GetRepository<DeputyAssociation>().GetById(singleDeputy.AssociationId);
                deputyModel.Association = association.Name;
            }

            if (multyDeputy != null)
            {
                deputyModel.Id = multyDeputy.Id;
                deputyModel.Name = multyDeputy.Name;
                deputyModel.Email = multyDeputy.Email;
                deputyModel.IsActive = multyDeputy.IsActive;
                deputyModel.Link = multyDeputy.Link;
                deputyModel.PhotoLink = multyDeputy.PhotoLink;
                deputyModel.Position = multyDeputy.Position;

                deputyModel.ChosenBy = "Загальнодержавному багатомандатному округу";
                var party = await this.unitOfWork.GetRepository<PoliticalParty>().GetById(multyDeputy.PartyId);
                deputyModel.Party = party.Name;

                var association = await this.unitOfWork.GetRepository<DeputyAssociation>().GetById(multyDeputy.AssociationId);
                deputyModel.Association = association.Name;
            }

            var inquries = await this.unitOfWork.GetRepository<Inqury>().SearchFor(x => x.AuthorId == deputyId || x.CoauthorIds.Contains(deputyId));
            var organizations = await this.unitOfWork.GetRepository<Destination>().GetAll();

            var group = inquries.GroupBy(x => organizations.FirstOrDefault(y => y.Id == x.DestinationId)?.Name).Select(x => new OrganizationsResponseItem()
            {
                Count = x.Count(),
                Name = x.Key
            }).OrderByDescending(x => x.Count).ToList();

            return new DeputyInfoModel
            {
                DeputyModel = deputyModel,
                Organizations = group
            };
        }
    }
}

public class Pair<T, U>
{
    public Pair()
    {
    }

    public Pair(T first, U second)
    {
        this.First = first;
        this.Second = second;
    }

    public T First { get; set; }
    public U Second { get; set; }
};