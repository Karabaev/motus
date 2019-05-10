namespace Updater.Modules
{
    using InfoAgent;
    using NLog;
    using SerialService.DAL;
    using SerialService.DAL.Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SerialService.Infrastructure;
    using System.Threading.Tasks;
    using InfoAgent.Exceptions;
    using Shared;

    public class VideoMaterialBlockedCleanModule : BaseModule
    {
        private readonly InfoAgentService infoAgent = new InfoAgentService();
        private readonly IAppUnitOfWork unitOfWork = AppUnitOfWork.GetInstance();

        /// <summary>
        /// Изменить статус недоступных через API фильмов на Rejected
        /// </summary>
        private void Clean()
        {
            long count = 0;
            foreach(var material in FindBlocked())
            {
                var loadedMaterial = unitOfWork.VideoMaterials.GetLoaded(material.ID);
                if (loadedMaterial==null)
                {
                    continue;
                }

                loadedMaterial.CheckStatus = CheckStatus.Rejected;
                try
                {
                    if (unitOfWork.VideoMaterials.EditMaterial(loadedMaterial))
                    {
                        var message = $"[{loadedMaterial.Title}] помечен статусом [Rejected]";
                        Task.Run(() => this.logger.Info(message));
                        Console.WriteLine(message);
                    }
                    else
                    {
                        var message = $"[{loadedMaterial.Title}] Ошибка сохранения";
                        Task.Run(() => this.logger.Info(message));
                        Console.WriteLine(message);
                    }
                }
                catch (Exception ex)
                {
                    Task.Run(() => this.logger.Error(ex.Message));
                    Console.WriteLine($"Ошибка при попытке изменения материала ID-{loadedMaterial.ID}");
                    continue;
                }
                count++;
            }
            Task.Run(() => this.logger.Info($"Очистка зваершена. Очищено {count} елементов"));
        }

        /// <summary>
        /// Найти все заблокированные
        /// </summary>
        private IEnumerable<VideoMaterial> FindBlocked()
        {
            Console.WriteLine("Загрузка данных. Может занять значительное время");
            Task.Run(() => this.logger.Info("Старт загрузки"));

            var videoMaterials = 
                unitOfWork
                .VideoMaterials
                .GetAll()
                .Where
                (
                    vm => vm.CheckStatus == CheckStatus.Confirmed ||
                          vm.CheckStatus == CheckStatus.Checking
                );

            var result = new List<VideoMaterial>();
            foreach (var material in videoMaterials)
            {
                try
                {
                    infoAgent.GetFilmInfo(material.KinopoiskID);
                }
                catch(IsBlockedException ex)
                {
                    result.Add(material);
                }
                catch(Exception ex)
                {
                    Task.Run(() => this.logger.Error(ex.Message));
                    continue;
                }
            }

            Task.Run(() => this.logger.Info($"Загрузка завершена.Включено {result.Count} елементов"));

            return result;
        }

        public override void Launch()
        {
            Clean();
        }
    }
}
