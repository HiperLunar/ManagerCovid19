
var calendar = {
    date: new Date(),
    selectedDate: null,

    data: [],

    loadData: function () {},
    mark: function (date) { return false; },

    renderCalendar: function () {

        this.loadData(this.date.getMonth());

        this.date.setDate(1);

        const firstDayIndex = this.date.getDay();
        const firstDate = new Date(this.date.getFullYear(), this.date.getMonth(), 1 - firstDayIndex);

        const months = [
            "Janeiro",
            "Fevereiro",
            "Março",
            "Abril",
            "Maio",
            "Junho",
            "Julho",
            "Agosto",
            "Setembro",
            "Outubro",
            "Novembro",
            "Dezembro"
        ];

        document.querySelector(".date h1").innerHTML = months[this.date.getMonth()];
        document.querySelector(".date p").innerHTML = new Date().toDateString();

        $('.days').empty();

        while (firstDate.getMonth() <= this.date.getMonth() || firstDate.getDay() != 0) {
            var day = jQuery('<div>');
            day.html(firstDate.getDate());
            if (this.mark(firstDate)) day.addClass('marked');
            if (firstDate.getDate() == new Date().getDate()
                && firstDate.getMonth() == new Date().getMonth()
                && firstDate.getFullYear() == new Date().getFullYear())
                day.addClass('today');
            else if (firstDate.getMonth() < this.date.getMonth())
                day.addClass('prev-date');
            else if (firstDate.getMonth() > this.date.getMonth())
                day.addClass('next-date');
            if (this.selectedDate)
                if (firstDate.getFullYear() == this.selectedDate.getFullYear()
                    && firstDate.getMonth() == this.selectedDate.getMonth()
                    && firstDate.getDate() == this.selectedDate.getDate())
                    day.addClass('selected');

            day.attr('year', firstDate.getFullYear())
            day.attr('month', firstDate.getMonth())
            day.attr('date', firstDate.getDate())

            firstDate.setDate(firstDate.getDate() + 1);
            day.appendTo('.days');
        }
    }
}

$('.prev').click(() => {
    calendar.date.setMonth(calendar.date.getMonth() - 1)
    calendar.renderCalendar();
});

$('.next').click(() => {
    calendar.date.setMonth(calendar.date.getMonth() + 1)
    calendar.renderCalendar();
});

$('.days').click('div', element => {
    if (element.target.classList.contains('prev-date'))
        calendar.selectedDate = new Date(
            calendar.date.getFullYear(),
            calendar.date.getMonth() - 1,
            element.target.innerText
        );
    else if (element.target.classList.contains('next-date'))
        calendar.selectedDate = new Date(
            calendar.date.getFullYear(),
            calendar.date.getMonth() + 1,
            element.target.innerText
        );
    else
        calendar.selectedDate = new Date(
            calendar.date.getFullYear(),
            calendar.date.getMonth(),
            element.target.innerText
        );

    calendar.renderCalendar();
});

calendar.renderCalendar();