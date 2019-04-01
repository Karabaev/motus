namespace SerialService.App_Start
{
	using SerialService.DAL;
	using SerialService.Infrastructure.ElasticSearch;

	public static class ElasticIndex
    {
        public static void Index(IAppUnitOfWork unitOfWork)
        {
            var materialForIndex = unitOfWork.VideoMaterials.ElasticGetAll();
            MotusElasticsearch.Index(materialForIndex);
        }
	}
}