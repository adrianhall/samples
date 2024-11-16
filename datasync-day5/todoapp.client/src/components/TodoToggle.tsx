import { useTodos } from './TodoProvider.tsx';

export default function TodoToggle() {
  const { todos, toggleAllTodos } = useTodos();

  return (
    <>
      <input
        id="toggle-all"
        className="toggle-all"
        type="checkbox"
        checked={todos.every((todo) => todo.completed)}
        onChange={async (e) => await toggleAllTodos(e.target.checked)}
      />
      <label htmlFor="toggle-all">Mark all as completed</label>
    </>
  );
}