import {
    ReactNode,
    createContext,
    useContext,
    useEffect,
    useState
} from 'react';
import {
    Todo,
    TodoProviderState,
    VisibilityProviderState,
    VisibilityType
} from '../types';

const TodosContext = createContext<TodoProviderState>({
    todos: [],
    dispatch: () => null,
});

const VisibilityContext = createContext<VisibilityProviderState>({
    visibility: 'all',
    setVisibility: () => null,
});

export default function TodoProvider({ children }: { children: ReactNode }) {
    const [todos, setTodos] = useState<Todo[]>([]);
    const [visibility, setVisibility] = useState<VisibilityType>('all');

    /*
    ** Initializes the todos by loading from the API
    */
    useEffect(() => {
        async function fetchTodos() {
            try {
                const response = await fetch('/tables/todoitems');
                const data = await response.json();
                setTodos(data);
            } catch (error) {
                console.error('Failed to fetch todos', error);
            }
        }

        fetchTodos();
    }, []);

    const addTodo = async (title: string) => {
        try {
            const response = await fetch('/tables/todoitems', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    title,
                    completed: false
                }),
            });
            const newTodo = await response.json();
            setTodos((prevTodos) => [...prevTodos, newTodo]);
        } catch (error) {
            console.error('Failed to add todo', error);
        }
    };

    const removeTodo = async (id: string) => {
        try {
            await fetch(`/tables/todoitems/${id}`, {
                method: 'DELETE',
            });
            setTodos((prevTodos) => prevTodos.filter(todo => todo.id !== id));
        } catch (error) {
            console.error('Failed to remove todo', error);
        }
    };

    const editTodo = async (updatedTodo: Todo) => {
        try {
            const response = await fetch(`/tables/todoitems/${updatedTodo.id}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(updatedTodo),
            });
            const newTodo = await response.json();
            setTodos((prevTodos) => prevTodos.map((todo) => (todo.id === newTodo.id ? newTodo : todo)));
        } catch (error) {
            console.error('Failed to edit todo', error);
        }
    };

    const toggleAllTodos = async (checked: boolean) => {
        try {
            const response = await fetch('/api/todoitems/toggle-all', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ completed: checked }),
            });
            const updatedTodos = await response.json();
            setTodos(updatedTodos);
        } catch (error) {
            console.error('Failed to toggle all todos', error);
        }
    };

    const clearCompletedTodos = async () => {
        try {
            const response = await fetch('/api/todoitems/clear-completed', {
                method: 'POST',
            });
            const updatedTodos = await response.json();
            setTodos(updatedTodos);
        } catch (error) {
            console.error('Failed to clear completed todos', error);
        }
    };

    return (
        <TodosContext.Provider value={{ todos, addTodo, removeTodo, editTodo, toggleAllTodos, clearCompletedTodos }}>
            <VisibilityContext.Provider value={{ visibility, setVisibility }}>{children}</VisibilityContext.Provider>
        </TodosContext.Provider>
    );
};


export function useTodos() {
    return useContext(TodosContext)
}

export function useVisibility() {
    return useContext(VisibilityContext)
}