/**
 * Scheduled job area router implementation.
 *
 * @constructor
 * @extends {CollarRouter}
 */
var ScheduledJobsRouter = CollarRouter.extend({
    name: 'Schedules',
    routes: {
        'schedules/id/:id/jobs': 'index',
        'schedules/id/:id/jobs/id/:jid': 'id',
        'schedules/id/:id/jobs/q/:search': 'search',
        'schedules/id/:id/jobs/q/:search/id/:jid': 'searchId',
        'schedules/id/:id/jobs/p/:page': 'page',
        'schedules/id/:id/jobs/p/:page/id/:jid': 'pageId',
        'schedules/id/:id/jobs/q/:search/p/:page': 'searchPage',
        'schedules/id/:id/jobs/q/:search/p/:page/id/:jid': 'index'
    },

    /**
     * Initialization.
     *
     * @param {App} app The root application object.
     * @param {Object} options Initialization options.
     */
    initialize: function(app, options) {
        CollarRouter.prototype.initialize.call(this, app, options);
        this.controller = this.createController(ScheduledJobsController, this.options);
    },

    /**
     * Handles the ID route.
     *
     * @param {Number} id The requested schedule ID.
     * @param {Number} jid The requested record ID.
     */
    id: function(id, jid) {
        this.index(id, '', 1, jid, '');
    },

    /**
     * Handles the ID + action route.
     *
     * @param {Number} id The requested schedule ID.
     * @param {Number} jid The requested record ID.
     * @param {String} action The requested record action.
     */
    idAction: function(id, jid, action) {
        this.index(id, '', 1, jid, action);
    },

    /**
     * Handles the index route.
     *
     * @param {Number} id The requested schedule ID.
     * @param {String} search The requested search string.
     * @param {Number} page The requested page number.
     * @param {Number} jid The requested record ID.
     * @param {String} action The requested record action.
     */
    index: function(id, search, page, jid, action) {
        this.controller.index(
            decodeURIComponent((id || '').toString()),
            decodeURIComponent((search || '').toString()), 
            decodeURIComponent((page || '1').toString()), 
            decodeURIComponent((jid || '').toString()),
            decodeURIComponent((action || '').toString()));

        this.trigger('nav', this, {name: this.name});
    },

    /**
     * Handles the paging route.
     *
     * @param {Number} id The requested schedule ID.
     * @param {Number} page The requested page number.
     */
    page: function(id, page) {
        this.index(id, '', page, '', '');
    },

    /**
     * Handles paging + ID route.
     *
     * @param {Number} id The requested schedule ID.
     * @param {Number} search The requested page number.
     * @param {Number} jid The requested record ID.
     */
    pageId: function(id, page, jid) {
        this.index(id, '', page, jid, '');
    },

    /**
     * Handles paging + ID + action route.
     *
     * @param {Number} id The requested schedule ID.
     * @param {Number} search The requested page number.
     * @param {Number} jid The requested record ID.
     * @param {String} action The requested record action.
     */
    pageIdAction: function(id, page, jid, action) {
        this.index(id, '', page, jid, action);
    },

    /**
     * Handles the search route.
     *
     * @param {Number} id The requested schedule ID.
     * @param {String} search The requested search string.
     */
    search: function(id, search) {
        this.index(id, search, 1, '', '');
    },

    /**
     * Handles the search + ID route.
     *
     * @param {Number} id The requested schedule ID.
     * @param {String} search The requested search string.
     * @param {Number} jid The requested record ID.
     */
    searchId: function(id, search, jid) {
        this.index(id, search, 1, jid, '');
    },

    /**
     * Handles the search + ID + action route.
     *
     * @param {Number} id The requested schedule ID.
     * @param {String} search The requested search string.
     * @param {Number} jid The requested record ID.
     * @param {Action} action The requested record action.
     */
    searchIdAction: function(id, search, jid, action) {
        this.index(id, search, 1, jid, action);
    },

    /**
     * Handles the search + paging route.
     *
     * @param {Number} id The requested schedule ID.
     * @param {String} search The requested search string.
     * @param {Number} page The requested page number.
     */
    searchPage: function(id, search, page) {
        this.index(id, search, page, '', '');
    }
});