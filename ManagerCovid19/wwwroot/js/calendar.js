const date = new Date();
var selectedDate;

const renderCalendar = () => {
    date.setDate(1);

    const firstDayIndex = date.getDay();
    const firstDate = new Date(date.getFullYear(), date.getMonth(), 1 - firstDayIndex);

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

    document.querySelector(".date h1").innerHTML = months[date.getMonth()];
    document.querySelector(".date p").innerHTML = new Date().toDateString();

    $('.days').empty();

    while (firstDate.getMonth() <= date.getMonth() || firstDate.getDay() != 0) {
        var day = jQuery('<div>');
        day.html(firstDate.getDate());
        if (mark(firstDate)) day.addClass('marked');
        if (firstDate.getDate() == new Date().getDate()
            && firstDate.getMonth() == new Date().getMonth()
            && firstDate.getFullYear() == new Date().getFullYear())
            day.addClass('today');
        else if (firstDate.getMonth() < date.getMonth())
            day.addClass('prev-date');
        else if (firstDate.getMonth() > date.getMonth())
            day.addClass('next-date');
        if (selectedDate)
            if (firstDate.getFullYear() == selectedDate.getFullYear()
                && firstDate.getMonth() == selectedDate.getMonth()
                && firstDate.getDate() == selectedDate.getDate())
                day.addClass('selected');

        firstDate.setDate(firstDate.getDate() + 1);
        day.appendTo('.days');
    }
}

$('.prev').click(() => {
    date.setMonth(date.getMonth() - 1)
    renderCalendar();
});

$('.next').click(() => {
    date.setMonth(date.getMonth() + 1)
    renderCalendar();
});

$('.days').click('div', element => {
    if (element.target.classList.contains('prev-date'))
        selectedDate = new Date(
            date.getFullYear(),
            date.getMonth() - 1,
            element.target.innerText
        );
    else if (element.target.classList.contains('next-date'))
        selectedDate = new Date(
            date.getFullYear(),
            date.getMonth() + 1,
            element.target.innerText
        );
    else
        selectedDate = new Date(
            date.getFullYear(),
            date.getMonth(),
            element.target.innerText
        );
    renderCalendar();
});

renderCalendar();