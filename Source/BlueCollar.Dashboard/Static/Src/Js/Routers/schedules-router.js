/**
 * Schedules area router implementation.
 *
 * @constructor
 * @extends {CollarRouter}
 */
var SchedulesRouter = CollarRouter.extend({
    name: 'Schedules',
    routes: {
        'schedules': 'index',
        'schedules/id/:id': 'id',
        'schedules/q/:search': 'search',
        'schedules/q/:search/id/:id': 'searchId',
        'schedules/p/:page': 'page',
        'schedules/p/:page/id/:id': 'pageId',
        'schedules/q/:search/p/:page': 'searchPage',
        'schedules/q/:search/p/:page/id/:id': 'index'
    },

    /**
     * Initialization.
     *
     * @param {App} app The root application object.
     * @param {Object} options Initialization options.
     */
    initialize: function(app, options) {
        CollarRouter.prototype.initialize.call(this, app, options);
        this.controller = this.createController(SchedulesController, this.options);
    }
});