/**
 * Implements the scheduled jobs edit form.
 *
 * @constructor
 * @extends {FormView}
 */
var ScheduledJobsEditView = FormView.extend({
    serializers: {
        "Id": new IntFieldSerializer()
    },
    template: _.template($('#scheduled-jobs-edit-template').html()),
    validators: {

    },

    /**
     * Focuses the first element in the form.
     */
    focus: function() {
        this.$('input[name="JobType"]').focus();
        return this;
    }
});