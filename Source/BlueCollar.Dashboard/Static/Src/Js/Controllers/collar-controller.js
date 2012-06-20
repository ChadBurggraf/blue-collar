/**
 * Base controller implementation.
 *
 * @constructor
 * @param {jQuery} page A reference to the page jQuery element.
 * @param {Object} options Initialization options. 
 */
var CollarController = function(page, options) {
    this.page = page;
    this.options = _.extend({}, options);
    this.initialize(this, this.options);
};

/**
 * Static functions.
 */
_.extend(CollarController, {
    /**
     * Mixin extend functionality to enable inheritence.
     */
    extend: extend
});

/**
 * Prototype functions.
 */
_.extend(CollarController.prototype, Backbone.Events, {
    /**
     * Initialization.
     *
     * @param {Object} options Initialization options.
     */
    initialize: function(options) {}
});