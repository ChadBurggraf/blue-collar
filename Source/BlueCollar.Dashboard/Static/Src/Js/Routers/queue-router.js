/**
 * Queue area router implementation.
 *
 * @constructor
 * @extends {CollarRouter}
 */
var QueueRouter = CollarRouter.extend({
    routes: {
        'queue': 'index'
    },

    /**
     * Initialization.
     * @this {QueueRouter}
     * @param {App} app The root application object.
     * @param {Object} options Additional initialization options.
     */
    initialize: function(app, options) {
        CollarRouter.prototype.initialize.call(this, app, options);
        this.options = _.extend({}, options);
    },

    /**
     * Handles the root #queue route.
     * @this {QueueRouter}
     */
    index: function() {
        
    }
});