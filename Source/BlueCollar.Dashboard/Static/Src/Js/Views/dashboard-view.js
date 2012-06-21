/**
 * Manages the root dashboard view.
 *
 * @constructor
 */
var DashboardView = Backbone.View.extend({
    statsTemplate: _.template($('#dashboard-stats-template').html()),
    template: _.template($('#dashboard-template').html()),

    /**
     * Initialization.
     *
     * @param {Object} options Initialization options.
     */
    initialize: function(options) {
        this.model.bind('change', this.render, this);
    },

    /**
     * Renders the view.
     *
     * @return {DashboardView} This instance.
     */
    render: function() {
        var json = this.model.toJSON(),
            statsJson = _.extend(_.clone(json.HistoryStatusRecent), _.clone(json.Counts)),
            statsHtml,
            notSucceededCount,
            workingEl,
            succeededEl,
            notSucceededEl,
            totalEl;

        this.$el.html(this.template(json));

        statsHtml = this.$('.stats').html(this.statsTemplate(statsJson));
        notSucceededCount = statsJson.CanceledCount + statsJson.FailedCount + statsJson.InterruptedCount + statsJson.TimedOutCount;
        workingEl = $('<span/>').text(new Number(statsJson.WorkingCount).format('0,000'));
        succeededEl = $('<span/>').text(new Number(statsJson.SucceededCount).format('0,000'));
        notSucceededEl = $('<span/>').text(new Number(notSucceededCount).format('0,000'));
        totalEl = $('<span/>').text(new Number(statsJson.TotalCount).format('0,000'));

        statsHtml.find('.working-count').html(workingEl);
        statsHtml.find('.succeeded-count').html(succeededEl);
        statsHtml.find('.not-succeeded-count').html(notSucceededEl);
        statsHtml.find('.total-count').html(totalEl);

        if (statsJson.SucceededCount > 0) {
            succeededEl.addClass('green');
        }

        if (notSucceededCount > 0) {
            notSucceededEl.addClass('red');
        }

        this.renderStatusChart(this.$('.chart-job-status .chart-contents')[0], json.HistoryStatusDistant);
        this.renderWorkerLoadChart(this.$('.chart-worker-load .chart-contents')[0], json.JobsPerWorker);
        this.renderJobsPerHourChart(this.$('.chart-jobs-per-hour .chart-contents')[0], json.JobsPerHourByDay);

        return this;
    },

    /**
     * Renders jobs-per-hour chart.
     *
     * @param {HTMLElement} el The HTML element to render the chart into.
     * @param {Object} json The raw object representing the data to render.
     */
    renderJobsPerHourChart: function(el, json) {
        var data = new google.visualization.DataTable(),
            chart = new google.visualization.ColumnChart(el),
            queues,
            dates,
            prop,
            queueDays,
            dayQueues,
            day,
            cols,
            i,
            j,
            n,
            m;

        data.addColumn('string', 'Date');
        queues = _.groupBy(json, function(d) { return d.QueueName || '*'; });

        i = 1;
        for (prop in queues) {
            if (queues.hasOwnProperty(prop)) {
                queueDays = queues[prop];
                data.addColumn('number', prop);

                for (j = 0, m = queueDays.length; j < m; j++) {
                    queueDays[j].Index = i;
                }

                i++;
            }
        }

        cols = data.getNumberOfColumns();
        dates = _.groupBy(json, function(d) { return d.Date.toString('MMM d')});

        i = 0;
        for (prop in dates) {
            if (dates.hasOwnProperty(prop)) {
                dayQueues = dates[prop];
                data.addRow();
                data.setValue(i, 0, prop);

                for (j = 1; j < cols; j++) {
                    day = _.find(dayQueues, function(d) { return d.Index === j; });

                    if (day) {
                        data.setValue(i, j, day.JobsPerHour);
                    } else {
                        data.setValue(i, j, 0);
                    }
                }

                i++;
            }
        }

        chart.draw(data, {width:'100%', height:300, vAxis:{title:'Jobs per hour'}});
    },

    /**
     * Renders the status chart.
     *
     * @param {HTMLElement} el The HTML element to render the chart into.
     * @param {Object} json The raw object representing the data to render.
     */
    renderStatusChart: function(el, json) {
        var data = new google.visualization.DataTable(),
            chart = new google.visualization.PieChart(el);

        data.addColumn('string', 'Status');
        data.addColumn('number', 'Job Count');
        data.addRows(5);
        data.setValue(0, 0, 'Succeeded');
        data.setValue(0, 1, json.SucceededCount);
        data.setValue(1, 0, 'Failed');
        data.setValue(1, 1, json.FailedCount);
        data.setValue(2, 0, 'Canceled');
        data.setValue(2, 1, json.CanceledCount);
        data.setValue(3, 0, 'Interrupted');
        data.setValue(3, 1, json.InterruptedCount);
        data.setValue(4, 0, 'Timed Out');
        data.setValue(4, 1, json.TimedOutCount);

        chart.draw(data, {width:'100%', height:300});
    },

    /**
     * Renders the worker load chart.
     *
     * @param {HTMLElement} el The HTML element to render the chart into.
     * @param {Object} json The raw object representing the data to render.
     */
    renderWorkerLoadChart: function(el, json) {
        var data = new google.visualization.DataTable(),
            chart = new google.visualization.PieChart(el),
            worker,
            i,
            n;

        data.addColumn('string', 'Worker');
        data.addColumn('number', 'Job Count');

        for (i = 0, n = json.length; i < n; i++) {
            worker = json[i];
            data.addRow([worker.Name + ' - ' + String.machineDisplay(worker.MachineName, worker.MachineAddress), worker.Count]);
        }

        chart.draw(data, {width:'!00%', height:300});
    }
});