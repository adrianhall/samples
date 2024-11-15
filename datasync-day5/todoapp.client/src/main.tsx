import ReactDOM from 'react-dom/client';

import TodoInput from './components/TodoInput.tsx';
import TodoList from './components/TodoList.tsx';
import TodoFooter from './components/TodoFooter.tsx';
import TodoProvider from './components/TodoProvider.tsx';

import 'todomvc-app-css/index.css';

const rootNode = document.getElementById('root')!;
ReactDOM.createRoot(rootNode).render(
  <TodoProvider>
    <div className="todoapp">
      <TodoInput />
      <TodoList />
      <TodoFooter />
    </div>
  </TodoProvider>
);