module('Models');

test('BlueCollarInitialize', function() {
    var attr = {String: 'hello', Date: '\/Date(1240718400000)\/'},
        model;

    model = new BlueCollarModel(attr);
    equal(_.isDate(model.get('Date')), true);
});

test('NavigationItemListGetCurrent', function() {
    var list = new NavigationItemList();
    equal(list.getCurrent(), null);

    list.reset(list.parse({HistoryCount: 1, QueueCount: 2}));
    equal(list.getCurrent(), list.at(0));
});

test('NavigationItemListParse', function() {
    var list = new NavigationItemList(),
        models = list.parse({
            HistoryCount: 1,
            QueueCount: 2,
            ScheduleCount: 3,
            WorkingCount: 4,
            WorkerCount: 5
        });

    equal(models.length, 6);
});

test('NavigationItemListSetCurrent', function() {
    var list = new NavigationItemList();
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