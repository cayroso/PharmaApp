﻿using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Web.BackgroundServices
{
    public class JobQueue<T>
    {
        private readonly ConcurrentQueue<T> _jobs = new ConcurrentQueue<T>();
        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);

        public void Enqueue(T job)
        {
            _jobs.Enqueue(job);
            _signal.Release();
        }

        public async Task<T> DequeueAsync(CancellationToken cancellationToken = default)
        {
            await _signal.WaitAsync(cancellationToken);
            _jobs.TryDequeue(out var job);
            return job;
        }
    }

    //public class MyJobBackgroundService : BackgroundService
    //{
    //    private readonly ILogger<MyJobBackgroundService> _logger;
    //    private readonly JobQueue<Trip> _queue;
    //    private readonly IServiceScopeFactory _serviceScopeFactory;

    //    public MyJobBackgroundService(ILogger<MyJobBackgroundService> logger, JobQueue<Trip> queue, IServiceScopeFactory serviceScopeFactory)
    //    {
    //        _logger = logger;
    //        _queue = queue;
    //        _serviceScopeFactory = serviceScopeFactory;
    //    }

    //    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    //    {
    //        while (!stoppingToken.IsCancellationRequested)
    //        {
    //            var job = await _queue.DequeueAsync(stoppingToken);

    //            using (var scope = _serviceScopeFactory.CreateScope())
    //            {
    //                var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    //                var trip = await appDbContext.Trips
    //                    .Include(e => e.Rider)
    //                        .ThenInclude(e => e.User)
    //                    .Include(e=> e.ExcludedDrivers)
    //                    .FirstOrDefaultAsync(e => e.TripId == job.TripId);

    //                //  get available drivers
    //                var drivers = await appDbContext.Drivers.Include(e => e.User)
    //                    .Where(e => !trip.ExcludedDrivers.Any(ed=> ed.DriverId== e.DriverId) 
    //                                &&  e.DriverId != trip.RiderId 
    //                                && e.Availability == Data.Enums.EnumDriverAvailability.Available)
    //                    .ToListAsync();

    //                if (trip != null && drivers.Any())
    //                {
    //                    // do stuff
    //                    _logger.LogInformation("Working on job {JobId}", job.TripId);

    //                    //  TODO: sort drivers by rider's preferrence
    //                    var driver = drivers.First();

    //                    trip.DriverId = driver.DriverId;
    //                    trip.Status = Data.Enums.EnumTripStatus.DriverAssigned;
    //                    trip.Timelines.Add(new TripTimeline
    //                    {
    //                        TripId = trip.TripId,
    //                        UserId = trip.RiderId,
    //                        Status = trip.Status,
    //                    });

    //                    driver.Availability = Data.Enums.EnumDriverAvailability.Unavailable;

    //                    var tripContext = scope.ServiceProvider.GetRequiredService<IHubContext<TripHub, ITripClient>>();

    //                    //  TODO: notify driver/rider that trip is assigned
    //                    var resp = new DriverAssigned.Response
    //                    {
    //                        TripId = trip.TripId,
    //                        DriverId = trip.DriverId,
    //                        DriverName = driver.User.FirstLastName,

    //                        RiderId = trip.RiderId,
    //                        RiderName = trip.Rider.User.FirstLastName
    //                    };

    //                    await tripContext.Clients.Users(new[] { trip.DriverId, trip.RiderId }).DriverAssigned(resp);

    //                    await appDbContext.SaveChangesAsync();
    //                }
    //                else
    //                {
    //                    //  no available drivers, put them back
    //                    if (!drivers.Any())
    //                    {
    //                        _queue.Enqueue(job);
    //                    }
    //                }
    //            }
    //            await Task.Delay(2000);
    //        }
    //    }
    //}
}
