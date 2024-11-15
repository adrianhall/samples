import { useTodos, useVisibility } from './TodoProvider.tsx';
import TodoToggle from './TodoToggle.tsx';
import TodoItem from './TodoItem.tsx';

export default function TodoList() {
    const { todos } = useTodos();
    const { visibility } = useVisibility();

    const filteredTodos = todos.filter((todo) => {
        switch (visibility) {
            case 'active':
                return !todo.completed;
            case 'completed':
                return todo.completed;
            default:
                return true;
        }
    });

    return (
        todos.length > 0 && (
            <main className="main">
                <TodoToggle />
                <ul className="todo-list">
                    {filteredTodos.map((todo) => (
                        <TodoItem key={todo.id} todo={todo} />
                    ))}
                </ul>
            </main>
        )
    );
}