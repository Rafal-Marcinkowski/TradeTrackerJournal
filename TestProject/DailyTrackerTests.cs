using DataAccess.Data;
using Infrastructure;
using Infrastructure.Events;
using Moq;
using SharedProject.Interfaces;
using SharedProject.Models;

namespace TestProject;

public class DailyTrackerTests
{
    private readonly Mock<ITransactionData> _mockTransactionData;
    private readonly Mock<IDailyDataProvider> _mockDailyDataProvider;
    private readonly Mock<IEventAggregator> _mockEventAggregator;
    private readonly Mock<IEventData> _mockEventData;
    private readonly DailyTracker _dailyTracker;

    public DailyTrackerTests()
    {
        _mockTransactionData = new Mock<ITransactionData>();
        _mockDailyDataProvider = new Mock<IDailyDataProvider>();
        _mockEventAggregator = new Mock<IEventAggregator>();
        _mockEventData = new Mock<IEventData>();

        _mockEventAggregator
            .Setup(e => e.GetEvent<TransactionAddedEvent>())
            .Returns(new Mock<TransactionAddedEvent>().Object);

        _mockEventAggregator
            .Setup(e => e.GetEvent<EventAddedEvent>())
            .Returns(new Mock<EventAddedEvent>().Object);

        _dailyTracker = new DailyTracker(
            _mockTransactionData.Object,
            _mockDailyDataProvider.Object,
            _mockEventAggregator.Object,
            _mockEventData.Object
        );
    }

    [Fact]
    public async Task StartTracker_WithNoTrackables_ShouldNotProcessAnything()
    {
        _mockTransactionData
            .Setup(t => t.GetAllTransactionsAsync())
            .ReturnsAsync([]);

        _mockEventData
            .Setup(e => e.GetAllEventsAsync())
            .ReturnsAsync([]);

        await _dailyTracker.StartTracker();

        _mockTransactionData.Verify(t => t.GetAllTransactionsAsync(), Times.Once);
        _mockEventData.Verify(e => e.GetAllEventsAsync(), Times.Once);
        _mockDailyDataProvider.Verify(d => d.InsertDailyDataAsync(It.IsAny<DailyData>()), Times.Never);
    }

    [Fact]
    public async Task StartTracker_WithTrackable_ShouldProcessIt()
    {
        var mockTrackable = new Mock<ITrackable>();
        mockTrackable.Setup(t => t.ID).Returns(1);
        mockTrackable.Setup(t => t.CompanyName).Returns("Test Company");
        mockTrackable.Setup(t => t.IsTracking).Returns(true);
        mockTrackable.Setup(t => t.EntryMedianTurnover).Returns(0);
        mockTrackable.Setup(t => t.EntryDate).Returns(DateTime.Now.Date);
        mockTrackable.Setup(t => t.IsClosed).Returns(false);
        mockTrackable.Setup(t => t.EntryPrice).Returns(100);

        var mockRecords = new List<DataRecord>
            {
                new() { Date = DateTime.Now.Date, Close = 105, Turnover = 1000 }
            };

        _mockDailyDataProvider
            .Setup(d => d.GetDailyDataForTransactionAsync(It.IsAny<int>()))
            .ReturnsAsync([]);

        _mockDailyDataProvider
            .Setup(d => d.GetDailyDataForEventAsync(It.IsAny<int>()))
            .ReturnsAsync([]);

        await _dailyTracker.StartTracker(mockTrackable.Object);

        _mockDailyDataProvider.Verify(
            d => d.InsertDailyDataAsync(It.Is<DailyData>(x =>
                (x.TransactionID == 1 || x.EventID == 1) &&
                x.ClosePrice == 105 &&
                x.Turnover == 1000)),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task OnTrackableAdded_WhenTrackerIsNotWorking_ShouldStartTracker()
    {
        var mockTrackable = new Mock<ITrackable>();
        mockTrackable.Setup(t => t.ID).Returns(1);
        mockTrackable.Setup(t => t.CompanyName).Returns("Test Company");
        mockTrackable.Setup(t => t.IsTracking).Returns(true);
        mockTrackable.Setup(t => t.EntryMedianTurnover).Returns(0);
        mockTrackable.Setup(t => t.EntryDate).Returns(DateTime.Now.Date);
        mockTrackable.Setup(t => t.IsClosed).Returns(false);
        mockTrackable.Setup(t => t.EntryPrice).Returns(100);

        var mockRecords = new List<DataRecord>
            {
                new() { Date = DateTime.Now.Date, Close = 105, Turnover = 1000 }
            };

        _mockDailyDataProvider
            .Setup(d => d.GetDailyDataForTransactionAsync(It.IsAny<int>()))
            .ReturnsAsync([]);

        _mockDailyDataProvider
            .Setup(d => d.GetDailyDataForEventAsync(It.IsAny<int>()))
            .ReturnsAsync([]);

        await _dailyTracker.OnTrackableAdded(mockTrackable.Object);

        _mockDailyDataProvider.Verify(
            d => d.InsertDailyDataAsync(It.Is<DailyData>(x =>
                (x.TransactionID == 1 || x.EventID == 1) &&
                x.ClosePrice == 105 &&
                x.Turnover == 1000)),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task OnTrackableAdded_WhenTrackerIsWorking_ShouldQueueTrackable()
    {
        _dailyTracker.StartTracker();
        var mockTrackable = new Mock<ITrackable>();

        await _dailyTracker.OnTrackableAdded(mockTrackable.Object);

        _mockDailyDataProvider.Verify(
            d => d.InsertDailyDataAsync(It.IsAny<DailyData>()),
            Times.Never
        );
    }
}
