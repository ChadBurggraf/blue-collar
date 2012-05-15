/**
 * Overall Dashboard view container.
 */
var DashboardView = Backbone.View.extend({
    tagName: 'div',
    template: _.template($('#dashboard-template').html()),
    statsTemplate: _.template($('#dashboard-stats-template').html()),

    initialize: function (options) {
        this.model.bind('change', this.render, this);
    },

    render: function () {
        var el = $(this.el).html(this.template()),
            json = this.model.toJSON(),
            statsJson = _.extend(_.clone(json.HistoryStatusRecent), _.clone(json.Counts)),
            statsHtml = el.find('.stats').html(this.statsTemplate(statsJson)),
            notSucceededCount = statsJson.CanceledCount + statsJson.FailedCount + statsJson.InterruptedCount + statsJson.TimedOutCount,
            workingEl = $('<span/>').text(new Number(statsJson.WorkingCount).format('0,000')),
            succeededEl = $('<span/>').text(new Number(statsJson.SucceededCount).format('0,000')),
            notSucceededEl = $('<span/>').text(new Number(notSucceededCount).format('0,000')),
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

        this.renderStatusChart(el.find('.chart.status .chart-contents')[0], json.HistoryStatusDistant);
        this.renderWorkerLoadChart(el.find('.chart.worker .chart-contents')[0], json.JobsPerWorker);
        this.renderJobsPerHourChart(el.find('.chart.jobs-per-hour .chart-contents')[0], json.JobsPerHourByDay);

        return this;
    },

    renderJobsPerHourChart: function (el, json) {
        var data = new google.visualization.DataTable(),
            chart = new google.visualization.ColumnChart(el),
            queues,
            prop,
            queueDays,
            day,
            i,
            j,
            n,
            m;

        data.addColumn('string', 'Date');

        queues = _.groupBy(json, function (d) { return d.QueueName || '*'; });
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

        for (i = 0, n = json.length; i < n; i++) {
            day = json[i];
            data.addRow();
            data.setValue(i, 0, day.Date.toString('MMM d'));
            data.setValue(i, day.Index, day.JobsPerHour);
        }

        chart.draw(data, { width: 840, height: 300, vAxis: { title: 'Jobs per hour'} });
    },

    renderStatusChart: function (el, json) {
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

        chart.draw(data, { width: 400, height: 300 });
    },

    renderWorkerLoadChart: function (el, json) {
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

        chart.draw(data, { width: 400, height: 300 });
    }
});