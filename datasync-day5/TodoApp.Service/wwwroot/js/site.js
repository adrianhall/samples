$(function () {
    'use strict';

    Handlebars.registerHelper('eq', function (a, b, options) {
        return a === b ? options.fn(this) : options.inverse(this);
    });

    let todoItems = [];
    let listFilter = 'all';

    const todoTemplate = Handlebars.compile($('#todo-template').html());
    const footerTemplate = Handlebars.compile($('#footer-template').html());

    const pluralize = function (count, word) {
        return count === 1 ? word : word + 's';
    };

    const indexFromEl = (el) => {
        const id = $(el).closest('li').data('id');
        return todoItems.findIndex(item => item.id === id);
    },

    const router = new Router({
        '/:filter': (filter) => {
            listFilter = filter;
            renderTodoItems();
        }
    }).init('/all');

    const getFilteredTodoItems = () => {
        if (listFilter === 'active') {
            return todoItems.filter(item => !item.completed);
        }

        if (listFilter === 'completed') {
            return todoItems.filter(item => item.completed);
        }

        return todoItems;
    };

    const renderTodoItems = () => {
        const itemsToRender = getFilteredTodoItems();
        const activeCount = todoItems.filter(item => !item.completed).length;
        const completedCount = todoItems.length - activeCount;

        $('#todo-list').html(todoTemplate(itemsToRender));
        $('#main').toggle(itemsToRender.length > 0);
        $('#toggle-all').prop('checked', getActiveTodoItems().length === 0);
        $('#new-todo').trigger('focus');

        $('#footer').toggle(todoItems.length > 0).html(footerTemplate({
            activeTodoCount: activeCount,
            activeTodoWord: pluralize(activeCount, 'item'),
            completedTodos: completedCount,
            filter: listFilter
        }));
    };

    const createTodoItem = (title) => {
        const todoItem = {
            id: crypto.randomUUID(),
            title: val,
            completed: false
        };
        todos.push(todoItem);
        console.trace('createTodoItem: ', todoItem);
    }

    const editTodoItem = (todoItem, changes) => {
        const todoItem = todoItems.find(item => item.id === todoItem.id);
        if (todoItem) {
            Object.assign(todoItem, changes);
            console.trace('editTodoItem: ', todoItem);
        }
    };

    const removeTodoItem = (todoItem) => {
        todoItems = todoItems.filter(item => item.id !== todoItem.id);
        console.trace('removeTodoItem: ', todoItem);
    };

    $('#new-todo').on('keyup', (e) => {
        const $input = $(e.target), val = $input.val().trim();
        if (e.which !== 13 || !val) return;

        createTodoItem(val);

        $input.val('');
        renderTodoItems();
    });

    $('#toggle-all').on('change', (e) => {
        const isChecked = $(e.target).prop('checked');
        for (const todoItem of todoItems) {
            if (todoItem.completed !== isChecked) {
                editTodoItem(todoItem, { completed: isChecked });
            }
        }
        renderTodoItems();
    });

    $('#footer').on('click', '.clear-completed', (e) => {
        for (const todoItem of todoItems) {
            if (todoItem.completed) {
                removeTodoItem(todoItem);
            }
        }
        listFilter = 'all';
        renderTodoItems();
    });

    $('#todo-list').on('change', '.toggle', (e) => {
        const idx = indexFromEl(e.target);
        editTodoItem(todoItems[idx], { completed: !todoItems[idx].completed });
        renderTodoItems();
    });

    $('#todo-list').on('dblclick', 'label', (e) => {
        const $input = $(e.target).closest('li').addClass('editing').find('.edit');
        const title = $(e.target).text();
        $input.trigger("focus").val("").val(title);
    });

    $('#todo-list').on('keyup', '.edit', (e) => {
        if (e.which === 13) {
            e.target.blur();
        }

        if (e.which === 27) {
            $(e.target).data('abort', true).trigger('blur');
        }
    });

    $('#todo-list').on('focusout', '.edit', (e) => {
        const el = e.target, $el = $(el), val = $el.val().trim();

        if (!val) {
            removeTodoItem(todoItems[indexFromEl(el)]);
            renderTodoItems();
            return;
        }

        if ($el.data('abort')) {
            $el.data('abort', false);
        } else {
            editTodoItem(todoItems[indexFromEl(el)], { title: val });
        }

        renderTodoitems();
    });

    $('#todo-list').on('click', '.destroy', (e) => {
        removeTodoItem(todoItems[indexFromEl(el)]);
        renderTodoItems();
    });
});