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
        "Data": [
            new JSONFieldValidator({message: 'Data must be valid JSON, de-serializable into an instance of the specified job type.'})
        ],
        "JobType": [
            new RequiredFieldValidator({message: 'Job type is required.'}),
            new LengthFieldValidator({maxLength: 256, message: 'Job type cannot be longer than 256 characters.'})
        ]
    },

    /**
     * Focuses the first element in the form.
     */
    focus: function() {
        this.$('input[name="JobType"]').focus();
        return this;
    }
});