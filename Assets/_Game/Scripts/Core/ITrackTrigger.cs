namespace RichRun
{
    /// <summary>
    /// Любой объект трассы, реагирующий на игрока: деньги, бутылки, ворота, финиш.
    /// Игрок делает ровно один TryGetComponent на триггер — без кастов и тегов.
    /// </summary>
    public interface ITrackTrigger
    {
        void OnPlayerEnter(PlayerController player);
    }
}
