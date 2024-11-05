namespace CustomIdGeneration
{
    public class CustomIdGenerator
    {
        readonly int _workerId;
        readonly int _processId;
        int _timeStamp;
        int _timeStampMasked;
        long _lastTick;
        int _sequence;

        SpinLock _spinLock;

        public CustomIdGenerator(int workerIndex = 0)
        {
            _spinLock = new SpinLock(false);

            var workerId = Provider.GetWorkerId(workerIndex);

            _workerId = (workerId[0] << 24) | (workerId[1] << 16) | (workerId[2] << 8) | workerId[3];
            _processId = (workerId[4] << 24) | (workerId[5] << 16);
        }

        public CustomId NewId()
        {
            var ticks = DateTime.UtcNow.Ticks;

            var isLocked = false;
            _spinLock.Enter(ref isLocked);

            if (ticks > _lastTick)
                UpdateTimeSequence(ticks);
            else if (_sequence == 65535) // 2^16 - 1.....increment ticks because of rollover
                UpdateTimeSequence(_lastTick + 1);

            var sequence = _sequence++;

            var a = _timeStamp;
            var b = _timeStampMasked;

            if (isLocked)
                _spinLock.Exit();

            return new CustomId(a, b, _workerId, _processId | sequence);
        }

        public ArraySegment<CustomId> NewId(CustomId[] ids, int index, int count)
        {
            if (index + count > ids.Length)
                throw new ArgumentOutOfRangeException(nameof(count));

            var ticks = DateTime.UtcNow.Ticks;

            var isLocked = false;
            _spinLock.Enter(ref isLocked);

            if (ticks > _lastTick)
                UpdateTimeSequence(ticks);

            var limit = index + count;
            for (var i = index; i < limit; i++)
            {
                if (_sequence == 65535) // 2^16 - 1.....increment ticks because of sequence rollover
                    UpdateTimeSequence(_lastTick + 1);

                ids[i] = new CustomId(_timeStamp, _timeStampMasked, _workerId, _processId | _sequence++);
            }

            if (isLocked)
                _spinLock.Exit();

            return new ArraySegment<CustomId>(ids, index, count);
        }

        void UpdateTimeSequence(long tick)
        {
            _timeStampMasked = (int)(tick & 0xFFFFFFFF);
            _timeStamp = (int)(tick >> 32);

            _sequence = 0;
            _lastTick = tick;
        }
    }
}
