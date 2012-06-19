var DashboardRouter = CollarRouter.extend({
    routes: {
        'dashboard': 'dashboard',
        '*path': 'dashboard'
    },

    initialize: function(app, options) {
        CollarRouter.prototype.initialize.call(this, app, options);
    },

    dashboard: function() {
        debugger;
    }
});