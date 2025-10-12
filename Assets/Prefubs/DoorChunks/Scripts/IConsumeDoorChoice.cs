namespace DoorChunks.Scripts
{
    internal interface IConsumeDoorChoice
    {
        void OnDoorChoice(DoorChunk doorChunk, bool right, int value, BonusTypes type);
    }
}
