import { useTodos } from './TodoProvider.tsx';
import TodoFilters from './TodoFilters.tsx';

export default function TodoFooter() {
    const { todos } = useTodos();
    const remaining = todos.filter((todo) => !todo.completed).length;

    return (
        todos.length > 0 && (
            <footer className="footer">
                <TodoCount remaining={remaining} />
                <TodoFilters />
                {todos.length > remaining && <ClearButton />}
            </footer>
        )
    );
}

function TodoCount({ remaining }: { remaining: number }) {
    return (
        <span className="todo-count">
            <strong>{remaining}</strong>
            <span>{remaining === 1 ? ' item' : ' items'} left</span>
        </span>
    );
}

function ClearButton() {
    const { clearCompletedTodos } = useTodos();

    return (
        <button className="clear-completed" onClick={async () => await clearCompletedTodos()}>
            Clear completed
        </button>
    );
}