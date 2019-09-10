namespace SerialService.App_Start
{
	using SerialService.DAL;
	using SerialService.Infrastructure.ElasticSearch;
    using System.Threading.Tasks;

    public static class ElasticIndex
    {
        public static async Task IndexAsync(IAppUnitOfWork unitOfWork)
        {
            var materialForIndex = unitOfWork.VideoMaterials.ElasticGetAll();
            await MotusElasticsearch.IndexAsync(materialForIndex);
        }
	}
}