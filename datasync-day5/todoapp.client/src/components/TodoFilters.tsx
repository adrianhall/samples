import { useVisibility } from './TodoProvider.tsx';
import { VisibilityType } from '../types';
import { clsx } from 'ts-clsx';

export default function TodoFilters() {
    const filters: VisibilityType[] = ['all', 'active', 'completed'];
    return (
        <ul className="filters">
            {filters.map((filter) => (
                <Filter key={filter} filter={filter} />
            ))}
        </ul>
    );
}

function Filter({ filter }: { filter: VisibilityType }) {
    const { visibility, setVisibility } = useVisibility();
    return (
        <li>
            <a className={clsx({ selected: visibility === filter })} onClick={() => setVisibility(filter)}>
                {filter.charAt(0).toUpperCase() + filter.slice(1)}
            </a>
        </li>
    );
}