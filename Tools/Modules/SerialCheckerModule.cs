namespace Tools.Modules
{
    using System;
    using System.Linq;
    using SerialService.DAL;
    using Shared;

    public class SerialCheckerModule : BaseModule
    {
        public override void Launch()
        {
            this.logger.Info("Запуск модуля {0}", this.Name);
            IAppUnitOfWork unitOfWork = (IAppUnitOfWork)NinjectDependencyResolver.Instance.GetService(
                                                                typeof(IAppUnitOfWork));

            int serialCount = 0;
            int filmCount = 0;
            int errorCount = 0;

            foreach (var video in unitOfWork.VideoMaterials.GetAll())
            {
                this.logger.Info("Проверка видеоматериала с ID {0} Title {1}", video.ID, video.Title);

                try
                {
                    if(video.SerialSeasons.GroupBy(ss => ss.SeasonNumber).Count().Equals(1)  // если все сезоны с одинаковым номером
                        && video.SerialSeasons.TrueForAll(ss => ss.EpisodesCount == 1)) // если у всех сезонов 1 серия
                    {
                        this.logger.Info("Это фильм");
                        video.IsSerial = false;
                        filmCount++;
                    }
                    else
                    {
                        this.logger.Info("Это сериал");
                        video.IsSerial = true;
                        serialCount++;
                    }

                    if (unitOfWork.VideoMaterials.Update(video.ID, video))
                        this.logger.Info("Видеоматериал изменен");
                    else
                        this.logger.Warn("Видеоматериал НЕ изменен");
                }
                catch (Exception ex)
                {
                    errorCount++;
                    this.logger.Error(ex, "Ошибка при проверке/изменении фильма");
                }


            }
            this.logger.Info("Модуль {0} закончил работу. Количество сериалов: {1}, фильмов {2}. Количество ошибок: {3}",
                this.Name, serialCount, filmCount, errorCount);
        }
    }
}
