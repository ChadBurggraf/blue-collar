/**
 * Dashboard URL router. Maintains all primary routes in the application.
 */
var Dashboard = Backbone.Router.extend({
    routes: {
        '': 'dashboard',
        'dashboard': 'dashboard',
        'history': 'history',
        'history/:search/p:page': 'history',
        'queue': 'queue',
        'queue/:search/p:page': 'queue',
        'schedules': 'schedules',
        'schedules/:search/p:page': 'schedules',
        'schedules/id:id': 'scheduledJobs',
        'schedules/id:id/:search/p:page': 'scheduledJobs',
        'workers': 'workers',
        'workers/:search/p:page': 'workers',
        'working': 'working',
        'working/:search/p:page': 'working'
    },

    initialize: function(app, options) {
        var navList;

        options = _.extend({
            stats: null,
            showCounts: true,
            testLink: false
        }, options);

        navList = new NavigationItemList();
        navList.urlRoot = app.urlRoot;
        navList.showCounts = options.showCounts;
        navList.testLink = options.testLink;
        navList.url = app.jsonUrlRoot + 'counts';

        this.nav = new NavigationView({collection: navList});
        
        if (options.stats && options.stats.Counts) {
            navList.reset(navList.parse(options.stats.Counts));
        } else {
            navList.fetch();
        }
        
        this.app = app;
        this.stats = options.stats;

        this.main = $('#main');
        this.forms = $('#forms');
    },

    counts: function(controller, counts) {
        this.nav.collection.reset(this.nav.collection.parse(counts));
    },

    createController: function(controller, fragment, options) {
        var controller = new controller(this, this.app.jsonUrlRoot + fragment, this.main, this.forms, options);
        controller.bind('counts', this.counts, this);
        return controller;
    },

    dashboard: function() {
        this.index(null, null, DashboardController, 'stats', 'Dashboard', {stats: this.stats});
        this.stats = null;
    },

    history: function(search, page) {
        this.index(search, page, HistoryController, 'history', 'History');
    },

    index: function(search, page, controller, fragment, navItem, options) {
        var params = this.listParams(search, page);
        this.createController(controller, fragment, options).index(params);
        this.nav.collection.setCurrent(navItem);
    },

    intParam: function(value) {
        if (!_.isNumber(value)) {
            if ($.isNumeric(value)) {
                value = parseInt(value, 10);  
                
                if (isNaN(value)) {
                    value = null;
                } 
            } else {
                value = null;
            }
        }

        return value;
    },

    listParams: function(search, page) {
        search = search || '';

        if ($.isNumeric(page)) {
            page = parseInt(page);
        } else {
            page = 1;
        }

        if (page < 1) {
            page = 1;
        };

        return {search: search, page: page};
    },

    queue: function(search, page) {
        this.index(search, page, QueuedController, 'queue', 'Queue');
    },

    scheduledJobs: function(id, search, page) {
        var params = this.listParams(search, page);
        id = this.intParam(id);

        if (!_.isNull(id) && id > 0) {
            this.createController(ScheduledJobController, 'schedules/' + id + '/jobs', {scheduleId: id}).index(params);
            this.nav.collection.setCurrent('Schedules');
        } else {
            this.navigate('schedules', true);
        }
    },

    schedules: function(search, page) {
        this.index(search, page, ScheduleController, 'schedules', 'Schedules');
    },

    workers: function(search, page) {
        this.index(search, page, WorkerController, 'workers', 'Workers');
    },

    working: function(search, page) {
        this.index(search, page, WorkingController, 'working', 'Working');
    }
});