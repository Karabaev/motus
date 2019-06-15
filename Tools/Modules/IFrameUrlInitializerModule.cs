namespace Tools.Modules
{
    using System;
    using InfoAgent;
    using InfoAgent.Moonwalk;
    using SerialService.DAL;
    using Shared;

    public class IFrameUrlInitializerModule : BaseModule
    {
        public override void Launch()
        {
            foreach (var item in unitOfWork.VideoMaterials.GetAll())
            {
                try
                {
                    this.logger.Info("Проверка видеоматериала с ID {0} Title {1}", item.ID, item.Title);

                    if (string.IsNullOrWhiteSpace(item.IframeUrl))
                    {
                        this.logger.Info("Загрузка информации о фильме с Кинопоиск ID {0}", item.KinopoiskID);
                        FilmInfo info = this.service.GetFilmInfo(item.KinopoiskID);
                        this.logger.Info("Информация о фильме с Кинопоиск ID {0} загружена", item.KinopoiskID);

                        if (info != null)
                        {
                            this.logger.Info("Получен iframeUrl {0}", info.IframeUrl);
                            item.IframeUrl = info.IframeUrl;

                            if (unitOfWork.VideoMaterials.Update(item.ID, item))
                                this.logger.Info("Видеоматериал изменен");
                            else
                                this.logger.Warn("Видеоматериал НЕ изменен");
                        }
                    }
                }
                catch(Exception ex) 
                {
                    this.logger.Error(ex, "Ошибка при обновлении IframeUrl у фильма с ID {0}", item.ID);
                }
                
            }
        }

        private readonly InfoAgentService service = new InfoAgentService();
        private readonly IAppUnitOfWork unitOfWork = AppUnitOfWork.GetInstance();
    }
}
