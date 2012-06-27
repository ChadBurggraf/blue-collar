module('Models');

test('CollarInitialize', function() {
    var attr = {String: 'hello', Date: '\/Date(1240718400000)\/'},
        model;

    model = new CollarModel(attr);
    equal(_.isDate(model.get('Date')), true);
});

test('NavCollectionGetCurrent', function() {
    var list = new NavCollection();
    equal(list.getCurrent(), null);

    list.reset(list.parse({HistoryCount: 1, QueueCount: 2}));
    equal(list.getCurrent(), list.at(0));
});

test('NavCollectionParse', function() {
    var list = new NavCollection(),
        models = list.parse({
            HistoryCount: 1,
            QueueCount: 2,
            ScheduleCount: 3,
            WorkingCount: 4,
            WorkerCount: 5
        });

    equal(models.length, 6);
});

test('NavCollectionSetCurrent', function() {
    var list = new NavCollection();
    list.reset(list.parse({
        HistoryCount: 1,
        QueueCount: 2,
        ScheduleCount: 3,
        WorkingCount: 4,
        WorkerCount: 5
    }));

    list.setCurrent(list.at(3).get('Name'));
    equal(list.getCurrent(), list.at(3));
});